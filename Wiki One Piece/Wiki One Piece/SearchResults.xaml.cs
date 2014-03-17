using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wiki_One_Piece.Data;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrat de recherche, consultez la page http://go.microsoft.com/fwlink/?LinkId=234240

namespace Wiki_One_Piece
{
    /// <summary>
    /// Cette page affiche les résultats d'une recherche globale effectuée dans cette application.
    /// </summary>
    public sealed partial class SearchResults : Wiki_One_Piece.Common.LayoutAwarePage
    {
        private List<SampleDataItem> searchResults = new List<SampleDataItem>();
        private List<SampleDataItem> searchResultsPersonnage = new List<SampleDataItem>();
        private List<SampleDataItem> searchResultsFruits = new List<SampleDataItem>();

        public SearchResults()
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
            var queryText = navigationParameter as String;

            // TODO: Application-specific searching logic.  The search process is responsible for
            //       creating a list of user-selectable result categories:
            //
            //       filterList.Add(new Filter("<filter name>", <result count>));
            //
            //       Only the first filter, typically "All", should pass true as a third argument in
            //       order to start in an active state.  Results for the active filter are provided
            //       in Filter_SelectionChanged below.

            searchResultsPersonnage = DataPersonnages.Search(queryText);
            searchResultsFruits = DataFruits.Search(queryText);
            searchResults.AddRange(searchResultsPersonnage);
            searchResults.AddRange(searchResultsFruits);

            var filterList = new List<Filter>();
            filterList.Add(new Filter("All", searchResults.Count(), true));
            filterList.Add(new Filter("Personnages",searchResultsPersonnage.Count()));
            filterList.Add(new Filter("Fruits",searchResultsFruits.Count()));

            // Communicate results through the view model
            this.DefaultViewModel["QueryText"] = '\u201c' + queryText + '\u201d';
            this.DefaultViewModel["Filters"] = filterList;
            this.DefaultViewModel["ShowFilters"] = filterList.Count > 1;
        }

        /// <summary>
        /// Invoqué lorsqu'un filtre est sélectionné à l'aide d'un contrôle ComboBox avec l'état d'affichage Snapped.
        /// </summary>
        /// <param name="sender">Instance ComboBox.</param>
        /// <param name="e">Données d'événement décrivant la façon dont le filtre sélectionné a été modifié.</param>
        void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determine what filter was selected
            var selectedFilter = e.AddedItems.FirstOrDefault() as Filter;
            if (selectedFilter != null)
            {
                // Mirror the results into the corresponding Filter object to allow the
                // RadioButton representation used when not snapped to reflect the change
                selectedFilter.Active = true;

                // TODO: Respond to the change in active filter by setting this.DefaultViewModel["Results"]
                //       to a collection of items with bindable Image, Title, Subtitle, and Description properties

                if (selectedFilter.Name == "All")
                {
                    this.DefaultViewModel["Results"] = searchResults;
                }
                else if (selectedFilter.Name == "Personnages")
                {
                    this.DefaultViewModel["Results"] = searchResultsPersonnage;
                }
                else
                {
                    this.DefaultViewModel["Results"] = searchResultsFruits;
                }

                // Ensure results are found
                object results;
                ICollection resultsCollection;
                if (this.DefaultViewModel.TryGetValue("Results", out results) &&
                    (resultsCollection = results as ICollection) != null &&
                    resultsCollection.Count != 0)
                {
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                    return;
                }
            }

            // Display informational text when there are no search results.
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        /// <summary>
        /// Invoqué lorsqu'un filtre est sélectionné à l'aide d'un RadioButton, lorsque l'état d'affichage n'est pas Snapped.
        /// </summary>
        /// <param name="sender">Instance RadioButton sélectionnée.</param>
        /// <param name="e">Données d'événement décrivant la façon dont le RadioButton a été sélectionné.</param>
        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            // Reflétez la modification dans le CollectionViewSource utilisé par le contrôle ComboBox correspondant
            // pour garantir que la modification soit reflétée lorsque l'état d'affichage a la valeur Snapped
            if (filtersViewSource.View != null)
            {
                var filter = (sender as FrameworkElement).DataContext;
                filtersViewSource.View.MoveCurrentTo(filter);
            }
        }

        /// <summary>
        /// Modèle d'affichage décrivant l'un des filtres disponibles pour l'affichage des résultats de recherche.
        /// </summary>
        private sealed class Filter : Wiki_One_Piece.Common.BindableBase
        {
            private String _name;
            private int _count;
            private bool _active;

            public Filter(String name, int count, bool active = false)
            {
                this.Name = name;
                this.Count = count;
                this.Active = active;
            }

            public override String ToString()
            {
                return Description;
            }

            public String Name
            {
                get { return _name; }
                set { if (this.SetProperty(ref _name, value)) this.OnPropertyChanged("Description"); }
            }

            public int Count
            {
                get { return _count; }
                set { if (this.SetProperty(ref _count, value)) this.OnPropertyChanged("Description"); }
            }

            public bool Active
            {
                get { return _active; }
                set { this.SetProperty(ref _active, value); }
            }

            public String Description
            {
                get { return String.Format("{0} ({1})", _name, _count); }
            }
        }

        private void Select_Item(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as SampleDataItem;
            this.Frame.Navigate(typeof(ItemDetailPage), item.UniqueId);
        }
    }
}
