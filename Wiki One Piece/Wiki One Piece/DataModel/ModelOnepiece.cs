using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Wiki_One_Piece.Data;
using Windows.UI.Popups;

namespace Wiki_One_Piece
{

    public class Initiator
    {
        List<Equipage> equipes;
        List<TypeFruitDemons> typesfruit;
        List<Personnage> personnages;
        List<FruitDemon> fruitdemons;



        static Initiator singleton;

        private Initiator()
        {
            equipes = new List<Equipage>();
            personnages = new List<Personnage>();
            typesfruit = new List<TypeFruitDemons>();
            fruitdemons = new List<FruitDemon>();
        }

        public static Initiator getInstance()
        {
            if (singleton == null)
            {
                singleton = new Initiator();
            }
            return singleton;
        }

        public async Task FromWebPersonnages(ObservableCollection<SampleDataGroup> allgroupes)
        {
            HttpClient client = new HttpClient();
            MessageDialog x = null;
            try
            {

                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                string urldist;
                if (loader.GetString("langue") == "fr-FR")
                    urldist = JSONRessources.PersonnagesFR;
                else urldist = JSONRessources.PersonnagesEN;

                string res = await client.GetStringAsync(urldist);
                List<String> urls = GetResponseApiFrom<List<String>>(res);



                foreach (string url in urls)
                {
                    var newequipe = await Equipage.FromWeb<Equipage>(url);


                    if (newequipe != null)
                    {
                        equipes.Add(newequipe);
                        foreach (Personnage p in newequipe.equipe)
                        {
                            personnages.Add(p);
                        }

                        allgroupes.Add(newequipe.toGroupView());
                    }

                }


            }
            catch (Exception ex)
            {
                x = new MessageDialog(ex.Message + "\n" + ex.InnerException.Message + "\n");
            }

            if (x != null)
                await x.ShowAsync();

        }




        public async Task FromWebFruits(ObservableCollection<SampleDataGroup> allgroupes)
        {
            HttpClient client = new HttpClient();
            MessageDialog x = null;
            try
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                string urldist;

                
                
                if (loader.GetString("langue") == "fr-FR")
                {
                    urldist = JSONRessources.FruitsFR;
                }
                else
                {
                    urldist = JSONRessources.FruitsEN;
                }

                string res = await client.GetStringAsync(urldist);
                List<String> urls = GetResponseApiFrom<List<String>>(res);



                foreach (string url in urls)
                {
                    var newequipe = await TypeFruitDemons.FromWeb<TypeFruitDemons>(url);


                    if (newequipe != null)
                    {
                        typesfruit.Add(newequipe);
                        foreach (FruitDemon p in newequipe.equipe)
                        {
                            fruitdemons.Add(p);
                        }

                        allgroupes.Add(newequipe.toGroupView());
                    }

                }
            }
            catch (Exception ex)
            {
                x = new MessageDialog(ex.Message + "\n" + ex.InnerException.Message + "\n");
            }

            if (x != null)
                await x.ShowAsync();

        }


        public List<SampleDataGroup> toViewPersonnages()
        {
            var groupes = new List<SampleDataGroup>();

            foreach (Equipage eq in equipes)
            {
                groupes.Add(eq.toGroupView());
            }

            return groupes;
        }

        public List<SampleDataGroup> toViewFruits()
        {
            var groupes = new List<SampleDataGroup>();

            foreach (TypeFruitDemons eq in typesfruit)
            {
                groupes.Add(eq.toGroupView());
            }

            return groupes;
        }

        public static T GetResponseApiFrom<T>(string str) where T : class
        {
            DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(T));
            MemoryStream stre = new MemoryStream(Encoding.UTF8.GetBytes(str));
            T a = s.ReadObject(stre) as T;
            return a;
        }




    }


    /// <summary>
    /// ITEMS GENERIC
    /// </summary>
    /// 
    [DataContract]
    public class ItemGenerique
    {
        [DataMember]
        public string UniqueId { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Subtitle { get; set; }
        [DataMember]
        public string ImagePath { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Content { get; set; }

        public override string ToString()
        {
            return UniqueId + " " + Title + " " + Subtitle + "\n" + ImagePath + "\n" + Description + " \n\n" + Content;
        }




        virtual public SampleDataItem toItemView(SampleDataGroup parrent)
        {
            var item = new SampleDataItem(UniqueId,
                        Title,
                        Subtitle,
                        ImagePath,
                        Description,
                        Content,
                        parrent);

            return item;

        }


        public static async Task<T> FromWeb<T>(string url) where T : class
        {
            HttpClient client = new HttpClient();

            string server = url;

            T result = default(T);

            MessageDialog x;
            try
            {
                string res = await client.GetStringAsync(server);
                result = Initiator.GetResponseApiFrom<T>(res);

                x = new MessageDialog(result.ToString());

            }
            catch (Exception ex)
            {
                x = new MessageDialog(ex.Message);
            }

            await x.ShowAsync();

            return result;
        }


    }




    /// <summary>
    /// Groupe Generique
    /// </summary>
    /// <typeparam name="Item"></typeparam>
    public class GroupGenerique<Item> where Item : ItemGenerique
    {
        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public List<Item> equipe = new List<Item>();
        public override string ToString()
        {
            string ret = UniqueId + " " + Title + " " + Subtitle + "\n" + ImagePath + "\n" + Description + " \n\n";

            foreach (Item p in equipe)
            {
                ret += p.ToString();
            }
            return ret;
        }




        public static async Task<T> FromWeb<T>(string url) where T : class
        {
            HttpClient client = new HttpClient();

            string server = url;

            T result = default(T);

            MessageDialog x = null;
            try
            {
                string res = await client.GetStringAsync(server);
                result = Initiator.GetResponseApiFrom<T>(res);

                //  x = new MessageDialog(result.ToString());

            }
            catch (Exception ex)
            {

                x = new MessageDialog(ex.Message + "\n" + ex.InnerException.Message + "\n");
            }

            if (x != null)
                await x.ShowAsync();

            return result;
        }


        public SampleDataGroup toGroupView()
        {
            var groupe = new SampleDataGroup(UniqueId,
                    Title,
                    Subtitle,
                    ImagePath,
                    Description, typeof(Item).Name);

            foreach (Item p in equipe)
            {
                groupe.Items.Add(p.toItemView(groupe));
            }


            return groupe;
        }

    }


}
