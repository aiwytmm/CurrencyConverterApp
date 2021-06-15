using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using System.ComponentModel;

namespace CurrencyConverterApp
{
    public class CurrencyInfoView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CurrencyInfo currencyInfo;
        public string ID
        {
            get
            {
                return currencyInfo.ID;
            }
            set
            {
                if (currencyInfo.ID != value)
                {
                    currencyInfo.ID = value;
                    OnPropertyChanged("ID");
                }
            }
        }
        public string NumCode
        {
            get
            {
                return currencyInfo.NumCode;
            }
            set
            {
                if (currencyInfo.NumCode != value)
                {
                    currencyInfo.NumCode = value;
                    OnPropertyChanged("NumCode");
                }
            }
        }
        public string ChacCode
        {
            get
            {
                return currencyInfo.ChacCode;
            }
            set
            {
                if (currencyInfo.ChacCode != value)
                {
                    currencyInfo.ChacCode = value;
                    OnPropertyChanged("ChacCode");
                }
            }
        }
        public int Nominal
        {
            get
            {
                return currencyInfo.Nominal;
            }
            set
            {
                if (currencyInfo.Nominal != value)
                {
                    currencyInfo.Nominal = value;
                    OnPropertyChanged("Nominal");
                }
            }
        }
        public string Name
        {
            get
            {
                return currencyInfo.Name;
            }
            set
            {
                if (currencyInfo.Name != value)
                {
                    currencyInfo.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public double Value
        {
            get
            {
                return currencyInfo.Value;
            }
            set
            {
                if (currencyInfo.Value != value)
                {
                    currencyInfo.Value = value;
                    OnPropertyChanged("Value");
                }
            }
        }
        public double Previous
        {
            get
            {
                return currencyInfo.Previous;
            }
            set
            {
                if (currencyInfo.Previous != value)
                {
                    currencyInfo.Previous = value;
                    OnPropertyChanged("Previous");
                }
            }
        }
        public CurrencyInfoView()
        {
            currencyInfo = new CurrencyInfo();
        }
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
