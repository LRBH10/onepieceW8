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

    
    
    public class Equipage : GroupGenerique<Personnage>    {}
    [DataContract]
    public class Personnage : ItemGenerique    {
        [DataMember]
        public string Apparence { get; set; }
        [DataMember]
        public string Personnalite { get; set; }
        [DataMember]
        public string Relation { get; set; }
        [DataMember]
        public string Competences { get; set; }
        [DataMember]
        public string Histoire { get; set; }
        [DataMember]
        public string Divers { get; set; }


        public Personnage(string unique,string t, string st, string im, string desc, string ct, string ap, string pers, string rel, string cm,string his, string div)
        {
            UniqueId = unique;
            Title = t;
            Subtitle = st;
            ImagePath = im;
            Description =desc;
            Content = ct;
            Apparence = ap;
            Personnalite = pers;
            Relation = rel;
            Competences = cm;
            Histoire = his;
            Divers = div;

            
        }

        public override SampleDataItem toItemView(SampleDataGroup parrent)
        {
            var item = new PersonnageData(UniqueId,
                        Title,
                        Subtitle,
                        ImagePath,
                        Description,
                        Content,
                        Apparence,
                        Personnalite,
                        Relation,
                        Competences,
                        Histoire,
                        Divers,
                        parrent);

            return item;

        }


        public static string ToJSON<T>(T obj) where T : class
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);


                StreamReader sr = new StreamReader(stream);
                stream.Position = 0;

                var res = sr.ReadToEnd();

                return res;
            }
        }

    }



     
}
