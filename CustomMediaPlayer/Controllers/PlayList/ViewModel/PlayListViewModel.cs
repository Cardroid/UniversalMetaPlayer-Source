using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

using CustomMediaPlayer.Core;

using GongSolutions.Wpf.DragDrop;

using MahApps.Metro.Controls.Dialogs;

namespace CustomMediaPlayer.Controllers.PlayList
{
  public class PlayListPageViewModel : INotifyPropertyChanged, IDropTarget
  {
    //public delegate void PlayListChangedHandler();
    //public event PlayListChangedHandler PlayListRefreshNeed;
    public event PropertyChangedEventHandler PropertyChanged;
    public void Notify(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); 

    #region Drag & Drop
    /// <summary>
    /// 드롭 작업처리 상태
    /// </summary>
    private enum NowDropStatus
    {
      Ready, // 준비
      Complete, // 작업완료
      Cloning, // 복제중
      Working, // 작업 중
      Applying // 적용 중
    }

    BackgroundWorker FileDropWorker;

    // 드롭 오버
    public void DragOver(IDropInfo dropInfo)
    {
      if (MainWindow.PlayList.Playlist.NowWorking)
      {
        dropInfo.Effects = DragDropEffects.None;
        return;
      }
      if (dropInfo.Data is DataObject item && item.ContainsFileDropList())
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Link;
      }
      else if (dropInfo.DragInfo.PositionInDraggedItem.Y < 25)
      {
        if (dropInfo.DragInfo.SourceItems != null)
        {
          dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
          dropInfo.Effects = DragDropEffects.Move;
        }
      }
      else { dropInfo.Effects = DragDropEffects.None; }
    }

    // 드롭
    public void Drop(IDropInfo dropInfo)
    {
      if (MainWindow.PlayList.Playlist.NowWorking)
        return;

      if (dropInfo.Data is DataObject item && item.ContainsFileDropList())
      {
        if (item.GetDataPresent(DataFormats.FileDrop))
        {
          FileDropWorker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = false };
          FileDropWorker.DoWork += FileDropWorker_DoWork;
          FileDropWorker.ProgressChanged += FileDropWorker_ProgressChanged;
          FileDropWorker.RunWorkerCompleted += FileDropWorker_RunWorkerCompleted;
          FileDropWorker.RunWorkerAsync(dropInfo);
        }
      }
      else
      {
        GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
        //PlayListRefreshNeed?.Invoke();
        Notify("PlayList");
      }
    }

    private void FileDropWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      ProgressPercent = e.ProgressPercentage;
      if (e.UserState != null) { WorkingStatus = (int)e.UserState; }
    }

    private void FileDropWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Notify("PlayList");
      MainWindow.PlayList.Playlist.NowWorking = false;
      ProgressPercent = 0;
      FileDropWorker?.Dispose();
    }

    private void FileDropWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      MainWindow.PlayList.Playlist.NowWorking = true;
      string[] files = null;
      int index;
      if (e.Argument is IDropInfo dropinfo)
      {// 파일 드롭일 때
        index = dropinfo.InsertIndex;
        files = (string[])(dropinfo.Data as DataObject).GetData(DataFormats.FileDrop);
      }
      else if (e.Argument is string[])
      {// 순서바꾸기일 때
        if (SelectItem == null)
          index = MainWindow.PlayList.Playlist.Count;
        else
          index = SelectItem.ID;
        files = (string[])e.Argument;
      }
      else
        return;
      FileDropWorker.ReportProgress(0, NowDropStatus.Cloning);
      PlayListInfo tmpplaylist = MainWindow.PlayList.Playlist.Clone() as PlayListInfo;
      FileDropWorker.ReportProgress(0, NowDropStatus.Working);
      for (int i = 0; i < files.Length; i++)
      {
        if (Utility.Utility.IsMedia(files[i]))
        {
          tmpplaylist.Insert(index + i, new MediaInfo(files[i]));
        }
        FileDropWorker.ReportProgress((i + 1) * 100 / files.Length);
      }
      FileDropWorker.ReportProgress(0, NowDropStatus.Applying);
      Application.Current.Dispatcher.Invoke(delegate
      {
        MainWindow.PlayList.Playlist = tmpplaylist;
        MainWindow.PlayList.Playlist.IDRefresh();
      });
      FileDropWorker.ReportProgress(0, NowDropStatus.Complete);
    }

    #endregion

    // 진행상황 업데이트 라벨
    private int _WorkingStatus = (int)NowDropStatus.Ready;
    public int WorkingStatus
    {
      get => _WorkingStatus;
      set
      {
        _WorkingStatus = value;
        Notify("ProgressLabel");
      }
    }
    public int _ProgressPercent { private set; get; } = 0;
    private int ProgressPercent
    {
      set
      {
        _ProgressPercent = value;
        Notify("ProgressLabel");
      }
    }
    public string ProgressLabel
    {
      get
      {
        switch (_WorkingStatus)
        {
          case (int)NowDropStatus.Ready:
            return "준비";
          case (int)NowDropStatus.Complete:
            return "완료";
          case (int)NowDropStatus.Cloning:
            return "복제 중...";
          case (int)NowDropStatus.Working:
            return $"작업 중... {_ProgressPercent}%";
          case (int)NowDropStatus.Applying:
            return $"적용 중...";
          default:
            return "";
        }
      }
    }

    // 플레이리스트 선택 항목
    public MediaInfo SelectItem { set; get; } = null;

    // 플레이리스트
    public PlayListInfo PlayList
    {
      get => MainWindow.PlayList.Playlist;
      set => MainWindow.PlayList.Playlist = value;
    }

    // 플레이리스트 이름
    public string PlayListName
    {
      get => PlayList.PlayListName;
      set
      {
        PlayList.PlayListName = value;
        Notify("PlayListName");
      }
    }

    // 플레이리스트 현재재생중

    // 테마
    public Brush BorderBrush 
    { get => ((MainWindow)System.Windows.Application.Current.MainWindow).BorderBrush; }
    private int _BorderThickness = 0;
    public int BorderThickness
    {
      get => _BorderThickness;
      set
      {
        _BorderThickness = value;
        Notify("BorderThickness");
      }
    }

    /// <summary>
    /// 파일 다이얼로그를 열어 파일을 플레이리스트에 추가합니다.
    /// </summary>
    public void AddMediaformFileDialog()
    {
      var utility = new Utility.Utility();
      var filenames = utility.OpenDialog();
      FileDropWorker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = false };
      FileDropWorker.DoWork += FileDropWorker_DoWork;
      FileDropWorker.ProgressChanged += FileDropWorker_ProgressChanged;
      FileDropWorker.RunWorkerCompleted += FileDropWorker_RunWorkerCompleted;
      FileDropWorker.RunWorkerAsync(filenames);
    }
  }
}
