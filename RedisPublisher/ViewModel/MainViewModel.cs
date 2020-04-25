using RedisPublisher.Commands;
using RedisPublisher.Enums;
using RedisPublisher.Model;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedisPublisher.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ConnectionMultiplexer redis;
        private RedisChannel redisChannel;

        //
        // View bindigs.
        //

        // Props.

        private int meterID;
        public int MeterID
        {
            get => meterID;
            set => SetField(ref meterID, value);
        }

        private bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            set => SetField(ref isConnected, value);
        }

        private string outputLog;
        public string OutputLog
        {
            get => outputLog;
            set => SetField(ref outputLog, value);
        }

        private string ip;
        public string IP
        {
            get => ip;
            set => SetField(ref ip, value);
        }

        private int port;
        public int Port
        {
            get => port;
            set => SetField(ref port, value);
        }

        private string subscribeChannel;
        public string SubscribeChannel
        {
            get => subscribeChannel;
            set => SetField(ref subscribeChannel, value);
        }


        private MessageModel message;
        public MessageModel Message
        {
            get => message;
            set => SetField(ref message, value);
        }

        // Commands

        private RelayCommand connectToRedisCommand;
        public RelayCommand ConnectToRedisCommand
        {
            get
            {
                return connectToRedisCommand ??
                    (connectToRedisCommand = new RelayCommand(obj =>
                    {
                        ConnectToRedis();
                    }));
            }
        }

        private RelayCommand disconnectFromRedisCommand;
        public RelayCommand DisconnectFromRedisCommand
        {
            get
            {
                return disconnectFromRedisCommand ??
                    (disconnectFromRedisCommand = new RelayCommand(obj =>
                    {
                        DisconnectFromRedis();
                    }));
            }
        }

        private RelayCommand readActiveAggImportCommand;
        public RelayCommand ReadActiveAggImportCommand
        {
            get
            {
                return readActiveAggImportCommand ??
                    (readActiveAggImportCommand = new RelayCommand(obj =>
                    {
                        SendMessage(EMercuryAttributes.ActiveAggImport);
                    }));
            }
        }

        private RelayCommand readDateTimeCommand;
        public RelayCommand ReadDateTimeCommand
        {
            get
            {
                return null;
            }
        }

        private RelayCommand readSerialCommand;
        public RelayCommand ReadSerialCommand
        {
            get
            {
                return null;

            }
        }

        private RelayCommand readQualityParamsCommand;
        public RelayCommand ReadQualityParamsCommand
        {
            get
            {
                return null;
            }
        }

        //
        // Base impl.
        //

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            IP = "172.16.50.4";
            Port = 6370;
            Message = new MessageModel();
            Message.Uuid = "0a09b593-4044-b879-e0be-bc21ff0758e0";
            Message.ChannelName = "priority";
            Message.ObjectTypeIds = "200051";
        }

        //
        // Private methods.
        //

        private void ConnectToRedis()
        {
            try
            {
                redis = ConnectionMultiplexer.Connect(IP + ":" + Port);
                IsConnected = redis.IsConnected;

                if (IsConnected)
                {
                    redisChannel = new RedisChannel(SubscribeChannel, RedisChannel.PatternMode.Auto);
                    ISubscriber subs = redis.GetSubscriber();
                    subs.Subscribe(redisChannel, new Action<RedisChannel, RedisValue>((channel, message) =>
                    {
                        OutputLog += message.ToString() + "\r\n";
                    }));
                }
                else
                    MessageBox.Show("Redis server is unavailable.");
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        private void DisconnectFromRedis()
        {
            if (!IsConnected)
                return;

            redis.Close();
            IsConnected = redis.IsConnected;
        }

        private void SendMessage(EMercuryAttributes readingAttribute)
        {
            try
            {
                //string message = meterID + ":" + (int)readingAttribute;
                string message = Message.ToString();
                //"{\"t\":1,\"id\":0,\"resp_ch\":priority,\"uuid_l\":0a09b593-4044-b879-e0be-bc21ff0758e0,\"ots\":[200051, 200061, 210100]}";

                IDatabase storage = redis.GetDatabase();
                storage.Publish(redisChannel, message);

                var subs = redis.GetSubscriber();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

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
