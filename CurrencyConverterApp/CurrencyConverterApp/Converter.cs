using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Settings;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CurrencyConverterApp
{
    public class Converter : INotifyPropertyChanged
    {
        private double FEntry { get; set; }
        public double FirstEntry
        {
            get
            {
                return FEntry;
            }
            set
            {
                if (FEntry != value)
                {
                    FEntry = value;
                    if (!lockerCalc && !checkLoad)
                    {
                        lockerCalc = true;
                        SecoundEntry = Calc1();
                        Save();
                    }
                    OnPropertyChanged("firstEntry");
                    lockerCalc = false;
                }
            }
        }
        private double SEntry { get; set; }
        public double SecoundEntry
        {
            get
            {
                return SEntry;
            }
            set
            {
                if (SEntry != value)
                {
                    SEntry = value;
                    if (!lockerCalc && !checkLoad)
                    {
                        lockerCalc = true;
                        FirstEntry = Calc2();
                        Save();
                    }
                    OnPropertyChanged("secoundEntry");
                    lockerCalc = false;
                }
            }
        }

        public bool lockRash;
        public DateTime Dt { get; set; }
        public DateTime Date
        {
            get
            {
                return Dt;
            }
            set
            {
                if (Dt != value)
                {
                    Dt = value;
                    OnPropertyChanged("date");
                }
            }
        }
        private string FItem { get; set; }
        public string FirstItem
        {
            get
            {
                return FItem;
            }
            set
            {
                if (FItem != value)
                {
                    FItem = value;
                    OnPropertyChanged("firstItem");
                }
            }
        }
        private string SItem { get; set; }
        public string SecoundItem
        {
            get
            {
                return SItem;
            }
            set
            {
                if (SItem != value)
                {
                    SItem = value;
                    OnPropertyChanged("secoundItem");
                }
            }
        }
        public ObservableCollection<CurrencyInfoView> List { get; set; }

        string url = "https://www.cbr-xml-daily.ru/daily_json.js";

        private string firstIndex;

        private string secoundIndex;

        public bool lockerCalc;

        static object Locker = new object();
        private DateTime LastDate { get; set; }

        public bool checkLoad;
        public Converter()
        {
            Date = DateTime.UtcNow;
            List = new ObservableCollection<CurrencyInfoView>();
            Load();
            AddToList(Request());
            LastDate = Date;
            FirstItem = firstIndex;
            SecoundItem = secoundIndex;
            Task.Run(Convert);
        }
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        List<JToken> Request()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = httpClient.GetAsync(url).Result;
            while (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                response = httpClient.GetAsync(url).Result;
                Thread.Sleep(50);
            }
            var content = response.Content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(content.Result);
            var values = jObject.Values().ToList();
            while (Date.Date < values[0].ToObject<DateTime>().Date)
            {
                response = httpClient.GetAsync("https:" + values[2].ToString()).Result;
                while (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var url2 = "https://www.cbr-xml-daily.ru/archive/"
                        + values[0].ToObject<DateTime>().Year
                        + "/"
                        + values[0].ToObject<DateTime>().Month
                        + "/"
                        + values[0].ToObject<DateTime>().Day
                        + "/daily_json.js";
                    response = httpClient.GetAsync(new Uri(url2)).Result;
                    values[0] = values[0].ToObject<DateTime>().AddDays(-1);
                    Thread.Sleep(50);
                }
                Thread.Sleep(50);
                content = response.Content.ReadAsStringAsync();
                jObject = JObject.Parse(content.Result);
                values = jObject.Values().ToList();
            }
            var str = jObject.SelectToken(@"$.Valute").Values();
            return str.ToList<JToken>();
        }
        void AddToList(List<JToken> yes)
        {
            if (List.Count == 0)
            {
                foreach (var item in yes)
                {
                    var itemInfo = JsonConvert.DeserializeObject<CurrencyInfo>(item.ToString());
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        List.Add(new CurrencyInfoView
                        {
                            ID = itemInfo.ID,
                            Name = itemInfo.Name,
                            ChacCode = itemInfo.ChacCode,
                            Nominal = itemInfo.Nominal,
                            NumCode = itemInfo.NumCode,
                            Previous = itemInfo.Previous,
                            Value = itemInfo.Value
                        });
                    });
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    List.Add(new CurrencyInfoView
                    {
                        Value = 1,
                        Name = "Российский рубль",
                        ChacCode = "RUB",
                        Nominal = 1
                    });
                });
                return;
            }

            for (var i = 0; i < yes.Count - 1; i++)
            {
                var itemInfo = JsonConvert.DeserializeObject<CurrencyInfo>(yes[i].ToString());
                lock (Locker)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        List[i] = new CurrencyInfoView
                        {
                            ID = itemInfo.ID,
                            Name = itemInfo.Name,
                            ChacCode = itemInfo.ChacCode,
                            Nominal = itemInfo.Nominal,
                            NumCode = itemInfo.NumCode,
                            Previous = itemInfo.Previous,
                            Value = itemInfo.Value
                        };
                    });
                    Thread.Sleep(50);
                }
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                List[yes.Count] = new CurrencyInfoView
                {
                    Value = 1,
                    Name = "Российский рубль",
                    ChacCode = "RUB",
                    Nominal = 1
                };
                Thread.Sleep(50);
            });

        }
        void Save()
        {
            CrossSettings.Current.AddOrUpdateValue("firstEntry", FirstEntry);
            CrossSettings.Current.AddOrUpdateValue("secoundEntry", SecoundEntry);
            CrossSettings.Current.AddOrUpdateValue("date", Date);
            CrossSettings.Current.AddOrUpdateValue("firstItem", FirstItem);
            CrossSettings.Current.AddOrUpdateValue("secoundItem", SecoundItem);
        }
        void Load()
        {
            checkLoad = true;
            FirstEntry = CrossSettings.Current.GetValueOrDefault("firstEntry", FirstEntry);
            SecoundEntry = CrossSettings.Current.GetValueOrDefault("secoundEntry", SecoundEntry);
            Date = CrossSettings.Current.GetValueOrDefault("date", Date);
            FirstItem = CrossSettings.Current.GetValueOrDefault("firstItem", FirstItem);
            SecoundItem = CrossSettings.Current.GetValueOrDefault("secoundItem", SecoundItem);
            firstIndex = FirstItem;
            secoundIndex = SecoundItem;
            checkLoad = false;
        }
        double Calc1()
        {
            double secoundEntry = List[System.Convert.ToInt32(FirstItem)].Value * FirstEntry
                          / (List[System.Convert.ToInt32(SecoundItem)].Value * List[System.Convert.ToInt32(FirstItem)].Nominal);
            secoundEntry = Math.Round(secoundEntry, 2);
            return secoundEntry;
        }
        double Calc2()
        {
            double firstEntry = List[System.Convert.ToInt32(SecoundItem)].Value * SecoundEntry
                            / (List[System.Convert.ToInt32(FirstItem)].Value * List[System.Convert.ToInt32(SecoundItem)].Nominal);
            firstEntry = Math.Round(firstEntry, 2);
            return firstEntry;
        }
        void Convert()
        {
            while (true)
            {
                if (LastDate.Date != Date.Date)
                {
                    firstIndex = FirstItem;
                    secoundIndex = SecoundItem;
                    var val = Request();
                    checkLoad = true;
                    AddToList(val);
                    checkLoad = false;
                    LastDate = Date;
                    FirstItem = firstIndex;
                    SecoundItem = secoundIndex;
                    SecoundEntry = Calc1();
                    Save();
                }

                if (FirstItem != firstIndex)
                {
                    firstIndex = FirstItem;
                    checkLoad = true;
                    SecoundEntry = Calc1();
                    checkLoad = false;
                }
                if (SecoundItem != secoundIndex)
                {
                    secoundIndex = SecoundItem;
                    checkLoad = true;
                    SecoundEntry = Calc1();
                    checkLoad = false;
                }
            }
        }
    }
}
