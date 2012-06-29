// Afterglow, a complete hardware and software Ambilight clone project.
// https://github.com/FrozenPickle/Afterglow

// The Arduino code herein is adapted from the Adalight code found here
// http://www.ladyada.net/make/adalight/

// Arduino code to accept data from a host computer and drive RGB LEDs 
// via the FastSPI library ( http://code.google.com/p/fastspi/ )
// supporting digital RGB LED pixels based on one of: 
//   LPD6803
//   TM1809
//   HL1606
//   595
//   WS2801
// The original code from Adalight was meant only for the WS2801.

// At the time of writing FastSPI does not compile under Arduino 1.0,
// a modified version of the project is included with Afterglow that
// has already been changed to compile correctly.

// * Streaming *
// Note: the original Adalight implementation streamed the data direct
// to the LEDs. The FastSPI library holds a reference to each LED's
// colour data and therefore uses more memory. This means that this 
// implementation can support fewer LEDs than the original implementation.
// This could be worked around by making modifications to the FastSPI
// library to accept streamed data instead of buffering.

// Some effort is put into avoiding buffer underruns (where the output
// side becomes starved of data).

// LED data and clock lines are connected to the Arduino's SPI output.
// On traditional Arduino boards, SPI data out is digital pin 11 and
// clock is digital pin 13.  On both Teensy and the 32u4 Breakout,
// data out is pin B2, clock is B1.  LEDs should be externally
// powered -- trying to run any more than just a few off the Arduino's
// 5V line is not a good idea. LED ground should also be connected
// to Arduino ground.

// For TM1809 based RGB LEDs please refer to the FastSPI documentation on
// pin configurations.

#include <FastSPI_LED.h>

// If defined, a test pattern will run on the first valid header
// sent to the Arduino that has a different LED count. The test pattern 
// will show each pixel one at a time in red, then green, then blue. 
// This can be helpful for testing the wiring and colour configuration.
#define TESTPATTERN

// Sometimes chipsets wire in a backwards sort of way, running the
// test pattern can help to work out if any changes are needed here
//struct CRGB { unsigned char b; unsigned char r; unsigned char g; };
struct RGB { unsigned char r; unsigned char g; unsigned char b; };
struct RGB *leds; // The leds values

// A 'magic word' (along with LED count & checksum) precedes each block
// of LED data.
// The magic word can be whatever sequence you like, but each character
// should be unique, and frequent pixel values like 0 and 255 are
// avoided -- fewer false positives.  The host software will need to
// generate a compatible header: immediately following the magic word
// are three bytes: a 16-bit count of the number of LEDs (high byte
// first) followed by a simple checksum value (high byte XOR low byte
// XOR 0x55). LED data follows, 3 bytes per LED, in order R, G, B,
// where 0 = off and 255 = max brightness.
static const uint8_t magic[] = {'G','l','o'};
#define MAGICSIZE  sizeof(magic)
#define HEADERSIZE (MAGICSIZE + 3)

#define MODE_HEADER 0
#define MODE_HOLD   1
#define MODE_DATA   2

// If no serial data is received for a while, the LEDs are shut off
// automatically.  This avoids the annoying "stuck pixel" look when
// quitting LED display programs on the host computer.
static const unsigned long serialTimeout = 15000; // 15 seconds

void setLEDChipSet()
{
  // Be sure to check FastSPI documentation for how to use with other chipsets http://code.google.com/p/fastspi/
  FastSPI_LED.setChipset(CFastSPI_LED::SPI_LPD6803);
  //FastSPI_LED.setChipset(CFastSPI_LED::SPI_TM1809); // See FastSPI documentation for pin configuration for TM1809 
                                                      // (setPinCount, setPin)
  //FastSPI_LED.setChipset(CFastSPI_LED::SPI_HL1606);
  //FastSPI_LED.setChipset(CFastSPI_LED::SPI_595);
  //FastSPI_LED.setChipset(CFastSPI_LED::SPI_WS2801);
}

void setup()
{
  // Note: the buffer is 256, and indexIn/Out are bytes that overflow back to 0.
  // This means there is no need to do any bounds checking on the indexes simplifying 
  // code and improving performance slightly.
  uint8_t
    buffer[256],       // serial read buffer
    indexIn       = 0, // write index for buffer
    indexOut      = 0, // read index for buffer
    mode          = MODE_HEADER, // loop mode
    hi, lo, chk, i, // hi + lo of led count, check digit, and indexer
    rgbIndex      = 0; // 0 = red, 1 = green, 2 = blue
  int16_t
    bytesBuffered = 0, // number of bytes buffered from serial
    hold          = 0, // time in micro seconds to wait
    oldLedCount   = 0,
    ledCount      = 0, // the total LED count
    ledIndex      = 0, // current LED index
    c; // char read from serial
  int32_t
    bytesRemaining;
  unsigned long
    startTime,
    lastByteTime,
    lastAckTime,
    t;

  Serial.begin(115200);
  Serial.print("Glo\n"); // Send ACK string to host

  startTime    = micros();
  lastByteTime = lastAckTime = millis();

  // Initialise for 100 LEDs and set all to black (doesn't hurt if there are less LEDs)
  // this gives us all LEDs off on startup
  oldLedCount   = 100;
  ledCount      = 100;
  FastSPI_LED.setLeds(ledCount);
  setLEDChipSet();
  FastSPI_LED.init();
  FastSPI_LED.start();
  leds = (struct RGB*)FastSPI_LED.getRGBData();
  memset(leds, 0, ledCount * 3);
  FastSPI_LED.show();
                
  // using infinite loop instead of loop() function to increase performance.
  for(;;) {

    // Check for serial input:
    t = millis();
    if((bytesBuffered < 256) && ((c = Serial.read()) >= 0)) {
      buffer[indexIn++] = c;
      bytesBuffered++;
      lastByteTime = lastAckTime = t; // Reset timeout counters
    } else {
      // No data received yet. Send an ACK packet to host once every second.
      if((t - lastAckTime) > 1000) {
        Serial.print("Glo\n"); // Send ACK string to host
        lastAckTime = t; // Reset counter
      }
      // If no data received for an extended time, turn off all LEDs.
      if((t - lastByteTime) > serialTimeout && ledCount > 0) {
        memset(leds, 0, ledCount * 3);
        FastSPI_LED.show();
        lastByteTime = t; // Reset counter
      }
    }

    switch(mode) {
      case MODE_HEADER:
        // In header-seeking mode.  Is there enough data to check?
        if(bytesBuffered >= HEADERSIZE) {
          // Check for a 'magic word' match.
          for(i=0; (i<MAGICSIZE) && (buffer[indexOut++] == magic[i++]););
          if(i == MAGICSIZE) {
            // Magic word matches.  Now how about the checksum?
            hi  = buffer[indexOut++];
            lo  = buffer[indexOut++];
            chk = buffer[indexOut++];
            if(chk == (hi ^ lo ^ 0x55)) {
              // Checksum looks valid.  Get 16-bit LED count
              // (# LEDs is always > 0) and multiply by 3 for R,G,B.
              oldLedCount = ledCount;
              ledCount = (long)hi + (long)lo;
              ledIndex = 0;
              bytesRemaining = 3L * ledCount;
              bytesBuffered -= 3;
              mode           = MODE_HOLD; // Proceed to wait mode
              
              // We need the ledCount before we can initialise FastSPI_LED. Do this every time the ledCount changes
              if (oldLedCount != ledCount)
              {
                if (oldLedCount != 0)
                {
                  FastSPI_LED.stop();
                  free(leds); // Ensure we free the memory used before resetting the LED count
                }
                FastSPI_LED.setLeds(ledCount);
                
                setLEDChipSet();
                
                FastSPI_LED.init();
                FastSPI_LED.start();
                leds = (struct RGB*)FastSPI_LED.getRGBData();
                
                // Test Pattern one at a time (red, then green, then blue)
                // Handy to check the CRGB field orders and FastSPI timing/config
#ifdef TESTPATTERN
                for(int j = 0; j < 3; j++) { 
                  for(int i = 0 ; i < ledCount; i++ ) {
                    memset(leds, 0, ledCount * 3);
                    switch(j) { 
                      case 0: leds[i].r = 255; break;
                      case 1: leds[i].g = 255; break;
                      case 2: leds[i].b = 255; break;
                    }
                    FastSPI_LED.show();
                    delay(10);
                  }
                }
#endif
              }
            } else {
              // Checksum didn't match; search resumes after magic word.
              indexOut  -= 3; // Rewind
            }
          } // else no header match.  Resume at first mismatched byte.
          bytesBuffered -= i;
        }
        break;
  
      case MODE_HOLD:
        // Underrun prevention delay.
        if((micros() - startTime) < hold) break; // Still holding; keep buffering
        mode = MODE_DATA; // ...and fall through to MODE_DATA (no break):
  
      case MODE_DATA:
        if(bytesRemaining > 0) {
          if(bytesBuffered > 0) {
            switch(rgbIndex++) {
              case 0:
                leds[ledIndex].r = buffer[indexOut++];
                break;
              case 1:
                leds[ledIndex].g = buffer[indexOut++];
                break;
              case 2:
                // Set the blue component, and increment LED index
                leds[ledIndex++].b = buffer[indexOut++];
                rgbIndex = 0; // Reset to red for next byte
                break;
            }
            
            bytesBuffered--;
            bytesRemaining--;
          }
          // If serial buffer is threatening to underrun, start
          // introducing progressively longer pauses to allow more
          // data to arrive (up to a point). This is not really important
          // unless the data is being streamed to the LEDs.
          if((bytesBuffered < 32) && (bytesRemaining > bytesBuffered)) {
            startTime = micros();
            hold = 100 + (32 - bytesBuffered) * 10;
            mode = MODE_HOLD;
  	  }
        } else {
          // End of data -- send new values to LEDs
          FastSPI_LED.show();
          mode = MODE_HEADER; // Begin next header search
        }
    } // end switch
  } // end for(;;)
}

void loop()
{
  // Not used.
}
