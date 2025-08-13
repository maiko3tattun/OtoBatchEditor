using Avalonia.Controls.Selection;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OtoBatchEditor.ViewModels
{
    public class OtoListViewModel : ViewModelBase
    {
        [Reactive] public ObservableCollectionExtended<string> OtoiniList { get; set; } = new();
        public SelectionModel<string> SelectedItems { get; } = new SelectionModel<string>();

        public OtoListViewModel()
        {
            OtoIni.SetOtoIniList(OtoiniList);
            ResetOtoList();
        }

        public void OnDropInis(string[] files)
        {
            foreach (string path in files)
            {
                // File
                if (Path.GetFileName(path) == "oto.ini")
                {
                    FileInfo file = new FileInfo(path);
                    if (file.Length == 0) // 中身が空ならスルー
                    {
                        continue;
                    }
                    if (!OtoiniList.Contains(path))
                    {
                        OtoiniList.Add(path);
                    }
                }

                // Directory
                if (Directory.Exists(path))
                {
                    string[] otos = Directory.GetFiles(path, "oto.ini", SearchOption.AllDirectories);
                    foreach (string otopath in otos)
                    {
                        FileInfo file = new FileInfo(otopath);
                        if (file.Length == 0) // 中身が空ならスルー
                        {
                            continue;
                        }
                        if (!OtoiniList.Contains(otopath))
                        {
                            OtoiniList.Add(otopath);
                        }
                    }
                }
            }
        }

        public void ResetOtoList()
        {
            OtoiniList.Clear();
            if (OtoIni.IsPlugin)
            {
                OtoiniList.Add(OtoIni.Args!);
            }
        }

        public void RemoveOto()
        {
            if (SelectedItems != null && SelectedItems.SelectedItems.Count > 0)
            {
                string[] items = SelectedItems.SelectedItems.Select(item => item!).ToArray();
                if (OtoIni.IsPlugin)
                {
                    foreach (string item in items)
                    {
                        if (OtoIni.Args != item)
                        {
                            OtoiniList.Remove(item);
                        }
                    }
                }
                else
                {
                    OtoiniList.Remove(items);
                }
            }
        }
    }
}
