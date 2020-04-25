using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RedisPublisher.Model
{
    public class MessageModel : INotifyPropertyChanged
    {
        private string uuid;
        public string Uuid
        {
            get => uuid;
            set => SetField(ref uuid, value);
        }

        private string channelName;
        public string ChannelName
        {
            get => channelName;
            set => SetField(ref channelName, value);
        }

        private string objectTypeIds;
        public string ObjectTypeIds
        {
            get => objectTypeIds;
            set => SetField(ref objectTypeIds, value);
        }

        private int id;
        public int Id
        {
            get => id;
            set => SetField(ref id, value);
        }

        public override string ToString()
        {
            string result =
               "{\"t\":\"1\",\"id\":" + "\"" + id + "\"" + ",\"resp_ch\":" + "\"" + ChannelName + "\"" + ",\"uuid_l\":" + "\"" + Uuid + "\"" + ",\"ots\":[" + ObjectTypeIds + "]}";

            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
