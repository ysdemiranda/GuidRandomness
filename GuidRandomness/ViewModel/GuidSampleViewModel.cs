using GuidRandomness.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Data;
using System.Windows.Input;

namespace GuidRandomness.ViewModel
{
    class GuidSampleViewModel : INotifyPropertyChanged
    {
        #region Elements
        private Timer _sampleTimer;

        private ObservableCollection<GuidSampleModel> _samples = new ObservableCollection<GuidSampleModel>();
        public IEnumerable<GuidSampleModel> Samples
        {
            get { return _samples; }
            set
            {
                _samples = (ObservableCollection<GuidSampleModel>)value;
                RaisePropertyChanged("Samples");
                RaisePropertyChanged("Average");
                RaisePropertyChanged("StandardDeviation");
            }
        }

        public WideGuid StandardDeviation
        {
            get
            {
                SegmentedGuid a = Samples.Where(x => x.Index == Head - 2).First().Content;
                SegmentedGuid b = Samples.Where(x => x.Index == Head - 1).First().Content;

                return new WideGuid
                {
                    segment_a = b.segment_a - a.segment_a,
                    segment_b = b.segment_b - a.segment_b,
                    segment_c = b.segment_c - a.segment_c,
                    segment_d = b.segment_d - a.segment_d,
                    segment_e = b.segment_e - a.segment_e
                };
            }
         }

        public WideGuid Average
        {
            get
            {
                WideGuid result = new WideGuid();
                foreach(GuidSampleModel s in Samples)
                {
                    result.segment_a += s.Content.segment_a;
                    result.segment_b += s.Content.segment_b;
                    result.segment_c += s.Content.segment_c;
                    result.segment_d += s.Content.segment_d;
                    result.segment_e += s.Content.segment_e;
                }

                result.segment_a /= Samples.Count();
                result.segment_b /= Samples.Count();
                result.segment_c /= Samples.Count();
                result.segment_d /= Samples.Count();
                result.segment_e /= Samples.Count();

                return result;
            }
        }

        private uint _size;
        public uint Size
        {
            get { return _size; }
            set
            {
                bool bHeadMoved = false;
                if (value <= 1 || value == _size || value >= 256)
                {
                    return;
                }
                if (value > _size)
                {
                    for (int i = 0; i < (value - _size); i++)
                    {
                        _samples.Add(new GuidSampleModel(_head++));
                    }
                    bHeadMoved = true;
                }
                else if (value < _size)
                {
                    for (int i = 0; i < (_size - value); i++)
                    {
                        _samples.RemoveAt(0);
                    }
                }
                _size = value;
                RaisePropertyChanged("Size");
                if (bHeadMoved)
                {
                    RaisePropertyChanged("Head");
                }
                Samples = _samples;
            }
        }

        private uint _head;
        public uint Head
        {
            get { return _head; }
        }

        private Boolean _autoSampleEnabled;
        public Boolean AutoSampleEnabled
        {
            get { return _autoSampleEnabled; }
            set
            {
                _autoSampleEnabled = value;
                RaisePropertyChanged("AutoSampleEnabled");
            }
        }
        #endregion

        public GuidSampleViewModel()
        {
            _head = 0;
            _autoSampleEnabled = false;
            _samples.Add(new GuidSampleModel(_head++));
            _samples.Add(new GuidSampleModel(_head++));
            _size = 2;

            _sampleTimer = new Timer(1000);
            _sampleTimer.Elapsed += _sampleTimerCallback;
            _sampleTimer.AutoReset = true;
            _sampleTimer.Enabled = _autoSampleEnabled;

            BindingOperations.EnableCollectionSynchronization(Samples, this);
        }
        private void _sampleTimerCallback(Object source, ElapsedEventArgs e)
        {
            if(CanSampleNextExecute())
                SampleNextExecute(null);
        }

        #region ICommand implementations
        void SampleNextExecute(object parameter)
        {
            _samples.RemoveAt(0);
            _samples.Add(new GuidSampleModel(_head++));
            Samples = _samples;
        }
        Boolean CanSampleNextExecute()
        {
            return true;
        }
        public ICommand SampleNext 
        { 
            get 
            { 
                return new RelayCommand(SampleNextExecute, CanSampleNextExecute);
            }
        }

        void ToggleAutoExecute(object parameter)
        {
            _sampleTimer.Enabled = (Boolean)parameter;
            AutoSampleEnabled = (Boolean)parameter;
        }
        Boolean CanToggleAutoExecute()
        {
            return true;
        }
        public ICommand ToggleAuto
        {
            get
            {
                return new RelayCommand(ToggleAutoExecute, CanToggleAutoExecute);
            }
        }

        void ResizeSetExecute(object parameter)
        {
            switch((string)parameter)
            {
                case "LessSamples":
                    Size--;
                    break;
                case "MoreSamples":
                    Size++;
                    break;
                default:
                    break;
            }
        }
        Boolean CanResizeSetExecute()
        {
            return true;
        }
        public ICommand ResizeSet
        {
            get
            {
                return new RelayCommand(ResizeSetExecute, CanResizeSetExecute);
            }
        }
        #endregion

        #region INotifyPropertyChanged implementations
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
