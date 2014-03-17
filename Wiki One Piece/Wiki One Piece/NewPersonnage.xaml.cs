using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Page de base, consultez la page http://go.microsoft.com/fwlink/?LinkId=234237

namespace Wiki_One_Piece
{
    /// <summary>
    /// Page de base qui inclut des caractéristiques communes à la plupart des applications.
    /// </summary>
    public sealed partial class NewPersonnage : Wiki_One_Piece.Common.LayoutAwarePage
    {
        public NewPersonnage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Remplit la page à l'aide du contenu passé lors de la navigation. Tout état enregistré est également
        /// fourni lorsqu'une page est recréée à partir d'une session antérieure.
        /// </summary>
        /// <param name="navigationParameter">Valeur de paramètre passée à
        /// <see cref="Frame.Navigate(Type, Object)"/> lors de la requête initiale de cette page.
        /// </param>
        /// <param name="pageState">Dictionnaire d'état conservé par cette page durant une session
        /// antérieure. Null lors de la première visite de la page.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Conserve l'état associé à cette page en cas de suspension de l'application ou de la
        /// suppression de la page du cache de navigation. Les valeurs doivent être conformes aux
        /// exigences en matière de sérialisation de <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">Dictionnaire vide à remplir à l'aide de l'état sérialisable.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void reloadimage_click(object sender, RoutedEventArgs e)
        {
            Uri im = null;
            try
            {
                im = new Uri(ImageBox.Text, UriKind.Absolute);
            }
            catch
            {
            }

            if (im != null)
            {
                BitmapImage bitmapImage = new BitmapImage(im);
                ImagePersonnage.Source = bitmapImage;
            }

        }

        private async void sendemail_click(object sender, RoutedEventArgs e)
        {
            Personnage x = new Personnage(NameBox.Text, NameBox.Text, TitreBox.Text, ImageBox.Text, (string)RoleBox.SelectedItem, ContentBox.Text, AppranceBox.Text, PersonnaliteBox.Text, RelationBox.Text, CompetanceBox.Text, "", "");

            string s = Personnage.ToJSON<Personnage> (x);

            MessageDialog md = new MessageDialog(s);
            await md.ShowAsync();


            var mailto = new Uri("mailto:?to=wikionepiecewindows@gmail.com&subject=Add " + NameBox.Text + "&body=" + s);
            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }
    }
}
