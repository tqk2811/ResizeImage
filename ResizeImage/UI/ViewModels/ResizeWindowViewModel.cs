using TqkLibrary.WpfUi;

namespace ResizeImage.UI.ViewModels
{
  public class ResizeWindowViewModel : BaseViewModel
  {
    private double _Percent = 0;

    public double Percent
    {
      get { return _Percent; }
      set { _Percent = value; NotifyPropertyChange(); }
    }

    private string _Data = "12.4;22.8;50.8";

    public string Data
    {
      get { return _Data; }
      set { _Data = value; NotifyPropertyChange(); }
    }

    private int _Threads = 4;

    public int Threads
    {
      get { return _Threads; }
      set { _Threads = value; NotifyPropertyChange(); }
    }

    private string _RunButtonText = "Start";

    public string RunButtonText
    {
      get { return _RunButtonText; }
      set { _RunButtonText = value; NotifyPropertyChange(); }
    }
  }
}