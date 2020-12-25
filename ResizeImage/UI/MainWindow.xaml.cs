using ResizeImage.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TqkLibrary.Queues.TaskQueues;

namespace ResizeImage.UI
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly ResizeWindowViewModel resizeWindowViewModel;
    private readonly TaskQueue<ResizeImageQueue> resizeImages = new TaskQueue<ResizeImageQueue>();

    private int totalRun = 0;
    private int totalCompleted = 0;
    private string InputPath;
    private string OutputPath;

    public MainWindow()
    {
      resizeImages.Dispatcher = this.Dispatcher;
      resizeImages.OnQueueComplete += ResizeImages_OnQueueComplete;
      resizeImages.OnRunComplete += ResizeImages_OnRunComplete;
      resizeWindowViewModel = new ResizeWindowViewModel();
      this.DataContext = resizeWindowViewModel;
      InitializeComponent();
    }

    private void ResizeImages_OnRunComplete()
    {
      resizeWindowViewModel.Percent = 0;
      resizeWindowViewModel.RunButtonText = "Run";
    }

    private void ResizeImages_OnQueueComplete(ResizeImageQueue queue)
    {
      totalCompleted++;
      resizeWindowViewModel.Percent = totalCompleted * 100.0 / totalRun;
    }

    private void BT_Input_Click(object sender, RoutedEventArgs e)
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
        folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
        if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          InputPath = folderBrowserDialog.SelectedPath;
        }
      }
    }

    private void BT_Output_Click(object sender, RoutedEventArgs e)
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
        folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
        if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          OutputPath = folderBrowserDialog.SelectedPath;
        }
      }
    }

    private void BT_Process_Click(object sender, RoutedEventArgs e)
    {
      //check
      if (resizeImages.QueueCount == 0 && resizeImages.RunningCount == 0)
      {
        if (string.IsNullOrEmpty(resizeWindowViewModel.Data) || string.IsNullOrEmpty(InputPath) || string.IsNullOrEmpty(OutputPath) ||
        !Directory.Exists(InputPath) || !Directory.Exists(OutputPath))
        {
          System.Windows.MessageBox.Show("Chưa có input/output/chuỗi", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        List<double> vals = new List<double>();
        resizeWindowViewModel.Data.Split(';').Where(x => !string.IsNullOrEmpty(x)).ToList().ForEach(x =>
        {
          if (double.TryParse(x, out double result)) vals.Add(result);
        });

        if (vals.Count == 0)
        {
          System.Windows.MessageBox.Show("Chuỗi đầu vào rỗng", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        ResizeImageQueue.OutputFolder = OutputPath;
        ResizeImageQueue.SizePercents.Clear();
        ResizeImageQueue.SizePercents.AddRange(vals);
        resizeImages.ShutDown();

        var files = Directory.GetFiles(InputPath);
        foreach (var file in files) resizeImages.Add(new ResizeImageQueue(file));
        totalRun = resizeImages.QueueCount;
        totalCompleted = 0;
        resizeWindowViewModel.RunButtonText = "Stop";
        resizeImages.MaxRun = resizeWindowViewModel.Threads;
      }
      else
      {
        if (System.Windows.MessageBox.Show("Cancel?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) resizeImages.ShutDown();
      }
    }
  }
}