using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireDetectionSystem.ViewModels
{
    class ImageDetectionViewModel : IDialogAware
    {
        public DialogCloseListener RequestClose => throw new NotImplementedException();

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
