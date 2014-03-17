using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace Wiki_One_Piece
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Welcome : Page
    {
        static bool done = false;
        public Welcome()
        {
            this.InitializeComponent();


            if (!done)
            {
                share();
                actions();
                
            }
           
        }


        private async void actions()
        {
            //await Actions.DownloadFile();
            await Actions.InitUserInformation();
            
        }
        private void share()
        {
            SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;

            var dataTransferManager = DataTransferManager.GetForCurrentView();

            // register event to handle the share operation when it starts
            dataTransferManager.DataRequested += DataRequested;

            // show the charm bar with Share option opened
            //DataTransferManager.ShowShareUI();//*/

            done = true;

            //Actions.HttpUploadImage();
        }

        void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var privacyStatement = new SettingsCommand("privacy", "Privacy Statement", x => Windows.System.Launcher.LaunchUriAsync(new Uri("http://bibouh123.blog.com/2013/08/07/wiki-one-piece-policy/")));

            args.Request.ApplicationCommands.Clear();
            args.Request.ApplicationCommands.Add(privacyStatement);
        }


        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page. La propriété Parameter
        /// est généralement utilisée pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Personnge_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GroupedItemsPage), "Personnages");
        }

        private void Fruits_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GroupedItemsPage), "Fruits de Démon");
        }

        private void Invite_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewPersonnage));
        }

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            var dataPackage = e.Request.Data;
            dataPackage.Properties.Title = "Partage de l'Encyclopédie";
            dataPackage.Properties.Description = "Encyclopédie de One Piece";
            dataPackage.SetUri(new Uri("http://apps.microsoft.com/windows/fr-fr/app/wiki-one-piece/920efb9c-04c1-4d4b-b7eb-d4802f25c998", UriKind.Absolute));
        }
    }
}
