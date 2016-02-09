using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Models.Trophies;
using Template10.Mvvm;

namespace PlayStation_Gui.ViewModels
{
    public class TrophyViewModel : ViewModelBase
    {
        private Trophy _selectedTrophy;
        private bool _isOpen = default(bool);

        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                Set(ref _isOpen, value);
            }
        }

        public Trophy SelectedTrophy
        {
            get { return _selectedTrophy; }
            set
            {
                Set(ref _selectedTrophy, value);
            }
        }
    }
}
