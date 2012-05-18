using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Afterglow.Core.Extensions;
using Afterglow.Core.Configuration;
using System.Reflection;
using Afterglow.Core.Plugins;
using Afterglow.Core.Storage;
using Afterglow.Core.Log;
using System.Collections.ObjectModel;

namespace Afterglow.Core
{
    public class Settings : BaseRecord
    {
        public Settings(ITable table, Log.ILogger logger, AfterglowRuntime runtime) : base(table, logger, runtime)
        {
        }

        [ConfigTable(DisplayName = "Profiles", IsHidden = true)]
        public ObservableCollection<Profile> Profiles
        {
            get { return Get(() => Profiles); }
            set { Set(() => Profiles, value); }
        }

        public Profile AddProfile()
        {
            Profile profile = new Profile(Table.Database.AddTable(), this.Logger, this.Runtime);
            this.Profiles.Add(profile);
            SaveToStorage(() => this.Profiles, this.Profiles);
            return profile;
        }

        public void RemoveProfile(Profile profile)
        {
            this.Profiles.Remove(profile);
            SaveToStorage(() => this.Profiles, this.Profiles);
        }
    }
}
