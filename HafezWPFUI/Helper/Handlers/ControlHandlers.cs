using System.Windows;
using System.Windows.Input;
using HafezLibrary.DataAccess.Connector;

namespace HafezWPFUI.Helper.Handlers
{
    public static class ControlHandlers
    {
        public static bool IsKeyNumber(KeyEventArgs e)
        {
            var isKeyNumber = e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9;

            return isKeyNumber;
        }

        public static bool EnterPressed(KeyEventArgs e)
        {
            //todo change to enterPressed() and check before here
            if ( e.Key != Key.Enter )
                return false;

            var tRequest = new TraversalRequest(FocusNavigationDirection.Next);
            var keyboardFocus = Keyboard.FocusedElement as UIElement;
            keyboardFocus.MoveFocus(tRequest);
            return true;

        }

        public static void CheckChar(bool charMode, KeyEventArgs e)
        {
            //true for number
            if ( charMode )
            {
                if ( e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 )
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            //false for char
            else
            {
                if ( e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 )
                    e.Handled = true;
                else
                    e.Handled = false;
            }
        }

        /// <summary>
        /// This method checks Physical Key validation and if it wasn't connected it close the program
        /// </summary>
        public static void PhysicalKeyValidation()
        {

#if !DEBUG
            // return;
            if ( TinyxConnector.CheckConnection() )
                return;

            MessageBoxHandler.ShowMessage("خطا", "قفل سخت افزاری شناسایی نشد.\n" +
                                      "لطفا صحت اتصال را بررسی نمایید.\n" +
                                      "برنامه بسته میشود.");
            Application.Current.Shutdown();
#endif  

        }
    }
}