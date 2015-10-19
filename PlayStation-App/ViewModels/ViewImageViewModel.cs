using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Commands.Messages;
using PlayStation_App.Common;

namespace PlayStation_App.ViewModels
{
    public class ViewImageViewModel : NotifierBase
    {
        private MessageGroupItem _selectedItem;

        public MessageGroupItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
                OnPropertyChanged();
            }
        }

        public DownloadImage DownloadImage { get; set; } = new DownloadImage();
    }
}
