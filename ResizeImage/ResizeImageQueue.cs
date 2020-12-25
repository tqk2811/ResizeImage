using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Queues.TaskQueues;
using TqkLibrary.Media.Images;
using System.IO;
using System.Drawing.Imaging;

namespace ResizeImage
{
  internal class ResizeImageQueue : IQueue
  {
    public static string OutputFolder { get; set; }
    public static List<double> SizePercents { get; } = new List<double>();
    private readonly string imagePath;

    public ResizeImageQueue(string imagePath)
    {
      this.imagePath = imagePath;
    }

    private void Work()
    {
      FileInfo fileInfo = new FileInfo(imagePath);
      try
      {
        using Bitmap bitmap = (Bitmap)Bitmap.FromFile(imagePath);
        foreach (var size in SizePercents)
        {
          using Bitmap resize = bitmap.Resize(size / 100);
          resize.Save($"{OutputFolder}\\{fileInfo.Name}_{size}.png", ImageFormat.Png);
        }
      }
      catch (Exception)
      {
      }
    }

    #region IQueue

    public bool IsPrioritize => false;

    public bool ReQueue => false;

    public void Cancel()
    {
    }

    public bool CheckEquals(IQueue queue) => this.Equals(queue);

    public Task DoWork() => Task.Factory.StartNew(Work, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

    #endregion IQueue
  }
}