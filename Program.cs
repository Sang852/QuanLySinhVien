using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Drawing;
using System.Windows.Media;
using OfficeOpenXml.Style;
using System.Windows;


namespace QuanLySinhVien
{
    internal class Program
    {
        public enum Role
        {
            Admin,
            User
        }
        //Định nghĩa quyền hạn trong hệ thống quản lý sinh viên

        // Định nghĩa tài khoản
        struct Account
        {
            public string username;
            public string password;
            public Role role; // Kiểu quyền hạn
        }

        // Giả lập cơ sở dữ liệu lưu tài khoản mật khẩu của người dùng hoặc admin
        static Account[] accounts = new Account[100];
        static int count_acc = 0; // Biến kiểm soát số lượng account
        static Account? current_user = null; // Biến lưu trạng thái người đang đăng nhập là admin hay user

        static void Login()
        {
            Console.Clear();
            Console.WriteLine("ĐĂNG NHẬP HỆ THỐNG QUẢN LÝ SINH VIÊN");
            Console.Write("Nhập tên đăng nhập: ");
            string user = Console.ReadLine();

            Console.Write("Nhập mật khẩu: ");
            string pass = Console.ReadLine();

            //Kiểm tra tài khoản mật khẩu trong giả lập csdl có tài khoản trùng khớp không
            foreach (var account in accounts) { 
                if (account.username == user && account.password == pass) { 
                    current_user = account;
                    Console.WriteLine($"Đăng nhập thành công với quyền {account.role}");
                    return;
                }
            }

            Console.WriteLine("Sai tên đăng nhập hoặc mật khẩu.");
        }

        static void Logout()
        {
            current_user = null;
            Console.WriteLine("\tĐã đăng xuất.");
        }

        // Kiểm tra tài khoản có tồn tại trong cơ sở dữ liệu giả lập hay không
        static int Find_account(string username)
        {
            for (int i = 0; i < count_acc; i++)
            {
                if(accounts[i].username == username)
                {
                    return i;
                }
            }
            return -1;
        }

        //Reset mật khẩu bởi người dùng xác minh bằng mật khẩu cũ
        static void ResetPassword_User()
        {
            Console.Write("\tNhập username: ");
            string username = Console.ReadLine();

            int index = Find_account(username);
            if(index == -1)
            {
                Console.WriteLine("\tTài khoản không tồn tại.");
                return;
            }

            Console.Write("\tNhập mật khẩu cũ: ");
            string password = Console.ReadLine();
            if (accounts[index].password != password) {
                Console.WriteLine("\tMật khẩu cũ không đúng.");
                return;
            }

            Console.Write("\tNhập mật khẩu mới: ");
            string new_password = Console.ReadLine();

            accounts[index].password = new_password;
            Console.WriteLine("\tĐặt lại mật khẩu thành công.");

        }

        //Reset mật khẩu bởi admin
        static void Resetpassword_Admin()
        {
            Console.Write("\tNhập username cần reset: ");
            string username = Console.ReadLine();

            int index = Find_account(username);
            if(index == -1)
            {
                Console.WriteLine("\tTài khoản người dùng không tồn tại.");
                return;
            }

            Console.Write("\tNhập mật khẩu mới cho người dùng: ");
            string new_password = Console.ReadLine();
            accounts[index].password = new_password;
            Console.WriteLine($"\tĐã đặt lại mật khẩu cho tài khoản: {username}");
        }

        // Đăng ký tài khoản
        static void Register(Account[] accounts)
        {
            Console.WriteLine("ĐĂNG KÝ TÀI KHOẢN TRUY CẬP HỆ THỐNG QUẢN LÝ SINH VIÊN");
            Console.Write("Nhập username: ");
            string username = Console.ReadLine();

            if (Array.Exists(accounts, x => x.username == username))
            {
                Console.WriteLine("Tên đăng nhập đã tồn tại!");
                return;
            }

            string password, comfirmPassword;
            do
            {
                Console.Write("Nhập mật khẩu: ");
                password = Console.ReadLine();

                Console.Write("Nhập lại mật khẩu: ");
                comfirmPassword = Console.ReadLine();

                if (password != comfirmPassword)
                    Console.WriteLine("Mật khẩu không khớp, hãy nhập lại.");
            } while (password != comfirmPassword);

            Console.Write("Vai trò (1 - Admin, 2 - User): ");
            string role_input = Console.ReadLine();
            Role role = role_input == "1" ? Role.Admin : Role.Admin;

            // Thêm tài khoản mới vào mảng tài khoản
            accounts[count_acc] = new Account
            {
                username = username,
                password = password,
                role = role
            };
            count_acc++;

            if (accounts[count_acc - 1].role == Role.Admin)
            {
                SaveUsersToFileAdmin(accounts);
            }
            else if(accounts[count_acc - 1].role == Role.User) { SaveUsersToFileUser(accounts); }

            Console.WriteLine("Đăng ký thành công!");

        }
        // Lưu tất cả các tài khoản người dùng vào file users.txt
        static void SaveUsersToFileUser(Account[] accounts)
        {
            const string filePath = "D:\\CS\\QuanLySinhVien\\users.txt";
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                for(int i = 0; i < count_acc; i++)
                {
                    if (accounts[i].role == Role.User)
                    {
                        Account account = accounts[i];
                        sw.WriteLine($"{account.username};{account.password};{account.role}");
                    }
                }
            }
        }

        // Lưu tất cả các tài khoản admin vào file admin.txt
        static void SaveUsersToFileAdmin(Account[] accounts)
        {
            const string filePath = "D:\\CS\\QuanLySinhVien\\admin.txt";
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                for(int i = 0; i < count_acc; i++)
                { 
                    if (accounts[i].role == Role.Admin)
                    {
                        Account account = accounts[i];
                        sw.WriteLine($"{account.username};{account.password};{account.role}");
                    }
                }
            }
        }
        static void SaveAllAccounts()
        {
            SaveUsersToFileAdmin(accounts);
            SaveUsersToFileUser(accounts);
        }

        // hàm tải dữ liệu tài khoản từ file về
        static void LoadAllAccounts()
        {
            string[] paths = { "D:\\CS\\QuanLySinhVien\\users.txt", "D:\\CS\\QuanLySinhVien\\admin.txt" };
      
            count_acc = 0;
            foreach (string path in paths)
            {
                if (!File.Exists(path)) continue;
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split(';');
                    if(parts.Length == 3 && count_acc < accounts.Length)
                    {
                        accounts[count_acc] = new Account
                        {
                            username = parts[0],
                            password = parts[1],
                            role = (Role)Enum.Parse(typeof(Role), parts[2])
                        };
                        count_acc++;
                    }
                }
            }
            Console.WriteLine($"Đã tải tổng cộng {count_acc} tài khoản");

        }


        struct SinhVien
        {
            public string id; // mã sinh viên
            public string name;
            public int age;
            public string gender; // giới tính sinh viên
            public string Class; // mã lớp
            public float gpa; // gpa của sinh viên
        }
        static SinhVien[] DSSV = new SinhVien[100]; // Cấp phát mảng có 100 sinh viên để lưu thông tin của sinh viên
        static int count = 0; // Biến đếm số lượng sinh viên thực tế
        static void DocThongTin()
        {
            string file_path = "D:\\CS\\QuanLySinhVien\\data.txt";
            if(!(File.Exists(file_path))) //kiểm tra file sự tồn tại của file
            {
                Console.WriteLine("File đọc vào không tồn tại");
                // Gọi hàm tạo file và ghi dữ liệu buộc người dùng cung cấp dữ liệu khi file không tồn tại
                return;
            }
            using (StreamReader file = new StreamReader(file_path))
            {
                int i = 0;
                string line;
                while ((line = file.ReadLine()) != null && i < DSSV.Length)
                {
                    if (string.IsNullOrEmpty(line)) continue; // bỏ qua những dòng rỗng
                    string[] parts = line.Split(';'); // Tách chuỗi theo định dạng dấu ;
                    if (parts.Length < 6)
                    {
                        Console.WriteLine($"Dữ liệu dòng {i + 1} không hợp lệ {line}");
                        continue; // bỏ qua những dòng không đúng định dạng
                    }
                    SinhVien sv = new SinhVien();
                    //Kiểm tra khi thêm sinh viên mới có sự trùng lặp id hay không
                    sv.id = parts[0].Trim();
                    if(Array.Exists(DSSV, x => x.id == sv.id))
                    {
                        Console.WriteLine($"Mã sinh viên {sv.id} đã tồn tại ở dòng {i + 1}");
                        continue;
                    }

                    sv.name = parts[1].Trim();

                    if (!int.TryParse(parts[2].Trim(), out sv.age))
                    {
                        Console.WriteLine($"Tuổi không hợp lệ ở dòng {i + 1}");
                        sv.age = 0;
                    }

                    sv.gender = parts[3].Trim();
                    sv.Class = parts[4].Trim();

                    if (!float.TryParse(parts[5].Trim(), out sv.gpa))
                    {
                        Console.WriteLine($"GPA không hợp lệ ở dòng {i + 1}");
                        sv.gpa = 0;
                    }
                    DSSV[i] = sv;
                    count++;
                    i++;
                }
            }
        }    

        static void HienThiDanhSach(int soluongSV)
        {
            PrintBorderRow();
            Print_Row("Mã SV", "Tên", "Tuổi", "Giới tính", "Lớp", "GPA");
            PrintBorderRow();

            for (int i = 0; i < soluongSV; i++) {
                if (string.IsNullOrEmpty(DSSV[i].id)) continue;
                SinhVien sv = DSSV[i];
                Print_Row(sv.id, sv.name, sv.age.ToString(), sv.gender, sv.Class, sv.gpa.ToString());
            }

            PrintBorderRow();

        }

        //Hàm định dạng kẻ dọc của bảng
        static void Print_Row(string id, string name, string age, string gender, string Class, string gpa)
        {
            // Độ rộng các cột
            const int id_width = 17;
            const int name_width = 25;
            const int age_width = 11;
            const int gender_width = 15;
            const int class_width = 15;
            const int gpa_width = 11;

            string vertical = "|";
            Console.Write("\t");
            Console.Write(vertical + " " + id.PadRight(id_width - 1));
            Console.Write(vertical + " " + name.PadRight(name_width - 1));
            Console.Write(vertical + " " + age.PadRight(age_width - 1));
            Console.Write(vertical + " " + gender.PadRight(gender_width - 1));
            Console.Write(vertical + " " + Class.PadRight(class_width - 1));
            Console.Write(vertical + " " + gpa.PadRight(gpa_width - 1));
            Console.WriteLine(vertical);

        }

        //Hàm định dạng viền kẻ ngang của bảng
        static void PrintBorderRow()
        {
            const int id_width = 17;
            const int name_width = 25;
            const int age_width = 11;
            const int gender_width = 15;
            const int class_width = 15;
            const int gpa_width = 11;

            string corner = "+";
            string horizontal = "-";

            Console.Write('\t');
            Console.Write(corner + new string(horizontal[0], id_width));
            Console.Write(corner + new string(horizontal[0], name_width));
            Console.Write(corner + new string(horizontal[0], age_width));
            Console.Write(corner + new string(horizontal[0], gender_width));
            Console.Write(corner + new string(horizontal[0], class_width));
            Console.Write(corner + new string(horizontal[0], gpa_width));
            Console.WriteLine(corner);
        }




        // Hàm thêm sinh viên
        static void ThemSinhVien()
        {
            if(DSSV.Length == 0 || DSSV == null)
            {
                Console.WriteLine($"Danh sách sinh viên chưa được khởi tạo");
                return;
            }

            //Khởi tạo sinh viên mới
            SinhVien sv = new SinhVien();

            Console.InputEncoding = System.Text.Encoding.UTF8; // Đảm bảo mã hóa đầu vào
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Đảm bảo mã hóa đầu ra
            while(true)
            {
                Console.Write("\t\tNhập mã sinh viên: ");
                sv.id = Console.ReadLine();
                if(sv.id.Length == 8 && Array.Exists(DSSV, x => x.id == sv.id))
                {
                    Console.WriteLine("\t\tMã sinh viên phải có đủ 8 kí tự số và không được trùng nhau");
                    continue;
                }
                break;
            }

            Console.Write("\t\tNhập tên của sinh viên: ");
            sv.name = Console.ReadLine();

            Console.Write("\t\tNhập tuổi của sinh viên: "); 
            while(!int.TryParse(Console.ReadLine
                (), out sv.age) || sv.age <= 0)
            {
                Console.Write("\t\tTuổi sinh viên không hợp lệ. Nhập lại tuổi sinh viên: ");
            }
            do
            {
                Console.Write("\t\tNhập giới tính của sinh viên: ");
                sv.gender = Console.ReadLine();
            } while ((sv.gender != "Nam" && sv.gender != "Nữ") && sv.gender.Length != 3);

            Console.Write("\t\tNhập mã lớp: "); sv.Class = Console.ReadLine();
            Console.Write("\t\tNhập gpa của sinh viên: ");
            while (!float.TryParse(Console.ReadLine(), out sv.gpa) || sv.gpa < 0 || sv.gpa > 10)
            {
                Console.Write("\t\tGPA không hợp lệ. Nhập lại gpa của sinh viên: ");
            }
            DSSV[count] = sv;
            count++;//sau khi thêm sinh viên vào danh sách sinh viên thì tăng số lượng sinh viên thực tế
            Console.WriteLine("\t\tĐã thêm sinh viên thành công.");
        }

        // Hàm sửa thông tin
        static void SuaThongTin()
        {
            Console.Write("\t\tNhập mã sinh viên cần sửa thông tin: ");
            string idCanSua = Console.ReadLine();

            int index = -1; // Cờ hiệu để tìm kiếm id phù hợp trong DSSV
            for (int i = 0; i < count; i++)
            {
                if (DSSV[i].id == idCanSua)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1) // Không tìm thấy id phù hợp thì dừng chương trình
            {
                Console.WriteLine($"\t\tKhông tìm thấy sinh viên có mã: {idCanSua}");
                return;
            }

            SinhVien sv = DSSV[index];
            try
            {
                Console.Write("\t\tNhập tên mới (Nhập Enter để giữ nguyên): ");
                string new_name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(new_name))
                {
                    sv.name = new_name;
                }

                Console.Write("\t\tNhập tuổi mới (Nhập Enter để giữ nguyên): ");
                string new_age = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(new_age))
                {
                    if (int.TryParse(new_age, out int age) && age > 0)
                    {
                        sv.age = age;
                    }
                    else Console.WriteLine("\t\tTuổi không hợp lệ giữ nguyên.");
                }

                Console.Write("\t\tNhập giới tính mới (Nam/Nữ, Enter để giữ nguyên): ");
                string new_gender = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(new_gender))
                {
                    if (new_gender == "Nam" || new_gender == "Nữ")
                        sv.gender = new_gender;
                    else
                        Console.WriteLine("\t\tGiới tính không hợp lệ, giữ nguyên");
                }

                Console.Write("\t\tNhập mã lớp mới (Enter để giữ nguyên): ");
                string new_class = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(new_class)) sv.Class = new_class;
                Console.Write("\t\tNhập GPA mới (Enter để giữ nguyên): ");
                string new_gpa = Console.ReadLine();
                if(!string.IsNullOrWhiteSpace(new_gpa))
                {
                    if (float.TryParse(new_gpa, out float gpa) && gpa >= 0 && gpa <= 10)
                        sv.gpa = gpa;
                    else Console.WriteLine("\t\tGPA không hợp lệ, giữ nguyên");
                }

                DSSV[index] = sv;
                Console.WriteLine("\t\tĐã cập nhật thông tin sinh viên thành công.");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"\t\tLỗi khi cập nhật thông tin {ex.Message}");
            }
        }

        //Hàm xóa thông tin
        static void XoaThongTin()
        {
            Console.Write("\t\tNhập mã sinh viên cần xóa: ");
            string idCanXoa = Console.ReadLine();

            int index = -1;
            for (int i = 0; i < count; i++) {
                if (DSSV[i].id == idCanXoa)
                {
                    index = i;
                    break;
                }
            }

            if(index == -1)
            {
                Console.WriteLine("\t\t Không tìm thấy mã sinh viên cần xóa thông tin.");
                return;
            }

            Console.Write("\t\t Bạn có chắc chắn muốn xóa sinh viên này? (y/n): ");
            string confirm = Console.ReadLine().ToLower();
            if (confirm != "y") {
                Console.WriteLine("\t\tĐã hủy thao tác xóa thông tin.");
                return;
            }

            for(int i = index; i < count; i++)
            {
                DSSV[i] = DSSV[i + 1];
            }
            DSSV[count - 1] = new SinhVien(); // Xóa sạch dữ liệu thừa của khi dồn các sinh viên lên một bậc
            count--;

            Console.WriteLine("\t\t Đã xóa sinh viên thành công.");


        }

        static void TimKiemTheoID()
        {
            string idCanTim;
            while (true)
            {
                Console.Write("\t\tNhập mã sinh viên cần tìm kiếm: ");
                idCanTim = Console.ReadLine();
                if (idCanTim.Length == 8) break;
            }

            int index = -1;
            for (int i = 0; i < count; i++)
            {
                if(DSSV[i].id == idCanTim)
                {
                    index = i;
                    break;
                }
            }

            if(index == -1)
            {
                Console.WriteLine($"\t\t\tKhông tìm thấy sinh viên có mã sinh viên {idCanTim}");
                return;
            }

            SinhVien sv = DSSV[index];
            PrintBorderRow();
            Print_Row("Mã SV", "Tên", "Tuổi", "Giới tính", "Lớp", "GPA");
            PrintBorderRow();
            Print_Row(sv.id, sv.name, sv.age.ToString(), sv.gender, sv.Class, sv.gpa.ToString());
            PrintBorderRow();
        }

        static void TimKiemTheoMaLop()
        {
            string idCanTim;
            Console.Write("\t\tNhập mã lớp cần tìm kiếm: ");
            idCanTim = Console.ReadLine();

            SinhVien[] list = new SinhVien[count]; // Danh sách lưu các sinh viên ở lớp có mã lớp giống mã lớp cần tìm
            int soLuong = 0;
            for (int i = 0; i < count; i++)
            {
                if (DSSV[i].Class == idCanTim)
                {
                    list[soLuong] = DSSV[i];
                    soLuong++;
                }
            }

            if (soLuong == 0)
            {
                Console.WriteLine($"\t\t\tKhông tìm thấy sinh viên ở lớp có mã {idCanTim}");
                return;
            }
            
            PrintBorderRow();
            Print_Row("Mã SV", "Tên", "Tuổi", "Giới tính", "Lớp", "GPA");
            PrintBorderRow();
            for (int i = 0; i < soLuong; i++) {
                if (string.IsNullOrEmpty(idCanTim)) continue;
                SinhVien sv = list[i];
                Print_Row(sv.id, sv.name, sv.age.ToString(), sv.gender, sv.Class, sv.gpa.ToString());
            }
            PrintBorderRow();
        }

        static void TimKiemTheoTen()
        {
            Console.Write("\t\tNhập tên sinh viên cần tìm: ");
            string tenCanTim = Console.ReadLine();

            SinhVien[] kq = new SinhVien[count];
            int soLuong = 0;

            for(int i = 0; i < count; i++)
            {
                if (DSSV[i].name.ToLower().Contains(tenCanTim.ToLower()))
                {
                    kq[soLuong] = DSSV[i];
                    soLuong++;
                }
            }

            if (soLuong == 0) {
                Console.WriteLine($"\t\t\t Không tìm thấy sinh viên có tên chứa \"{tenCanTim}\"");
                return;
            }
            PrintBorderRow();
            Print_Row("Mã SV", "Tên", "Tuổi", "Giới tính", "Lớp", "GPA");
            PrintBorderRow();
            for (int i = 0; i < soLuong; i++) {
                SinhVien sv = kq[i];
                Print_Row(sv.id, sv.name, sv.age.ToString(), sv.gender, sv.Class, sv.gpa.ToString());
            }
            PrintBorderRow();
        }

        // Tìm kiếm sinh viên có GPA theo khoảng điểm
        static void TimKiemTheoKhoangGPA()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.Write("\t\t\tNhập GPA thấp nhất: ");
            float gpaMin = float.Parse(Console.ReadLine());

            Console.Write("\t\t\tNhập GPA coa nhất: ");
            float gpaMax = float.Parse(Console.ReadLine());

            bool timThay = false;
            Console.WriteLine($"\t\tCác sinh viên có GPA trong khoảng [{gpaMin} - {gpaMax}]: ");
            PrintBorderRow();
            Print_Row("Mã lớp", "Tên", "Tuổi", "Giới tính", "Lớp", "GPA");
            PrintBorderRow();
            for (int i = 0; i < count; i++)
            {
                if (DSSV[i].gpa >= gpaMin && DSSV[i].gpa <= gpaMax)
                {
                    SinhVien sv = DSSV[i];
                    Print_Row(sv.id, sv.name, sv.age.ToString(), sv.gender, sv.Class, sv.gpa.ToString());
                    timThay = true;
                }
            }
            PrintBorderRow();

            if (!timThay)
            {
                Console.WriteLine($"\t\tKhông tìm thấy sinh viên nào có trong khoảng GPA [{gpaMin} - {gpaMax}]");
            }


        }

        static void TimKiemTheoGPA()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Write("\t\t\tNhập GPA cần tìm: ");
            if (double.TryParse(Console.ReadLine(), out double gpaCanTim) && gpaCanTim <= 0 && gpaCanTim > 10)
            {
                Console.WriteLine("\t\t\tGPA không hợp lệ.");
                return;
            }
            
            bool timThay = false;
            PrintBorderRow();
            Print_Row("Mã lớp", "Tên", "Tuổi", "Giới tính", "Lớp", "GPA");
            PrintBorderRow();
            for (int i = 0; i < count; i++) {
                if (Math.Abs(DSSV[i].gpa - gpaCanTim) < 0.2)
                {
                    SinhVien sv = DSSV[i];
                    Print_Row(sv.id, sv.name, sv.age.ToString(), sv.gender, sv.Class, sv.gpa.ToString());
                    timThay = true;
                }
            }
            PrintBorderRow();

            if (!timThay)
            {
                Console.WriteLine($"\t\tKhông tìm thấy sinh viên có GPA cần tìm = {gpaCanTim}");
            }

        }
        //Menu chức năng tìm kiếm
        static void VeMenuTimKiem()
        {
            Console.WriteLine("Chọn thao tác: 2");
            Console.WriteLine("\t[2] Tìm kiếm.");
            Console.WriteLine("\tDanh sách các thao tác tìm kiếm: ");
            Console.WriteLine("\t2.1 - Tìm kiếm thông tin sinh viên theo mã sinh viên.");
            Console.WriteLine("\t2.2 - Tìm kiếm thông tin sinh viên theo mã lớp.");
            Console.WriteLine("\t2.3 - Tìm kiếm thông tin sinh viên theo tên sinh viên");
            Console.WriteLine("\t2.4 - Tìm kiếm thông tin sinh viên theo khoảng GPA.");
            Console.WriteLine("\t2.5 - Tìm kiếm thông tin sinh viên theo GPA.");
            Console.WriteLine("\tx - Thoát.");
        }


        static void TimKiem()
        {
            while (true)
            {
                //VeMenuTimKiem();
                Console.Clear();
                VeMenuChinh(current_user.Value.role); // Hiện menu chính theo quyền
                VeMenuTimKiem();
                //Console.WriteLine("\t\tDanh sách các thao tác tìm kiếm: ");
                //Console.WriteLine("\t\t2.1 - Tìm kiếm thông tin sinh viên theo mã sinh viên.");
                //Console.WriteLine("\t\t2.2 - Tìm kiếm thông tin sinh viên theo mã lớp.");
                //Console.WriteLine("\t\t2.3 - Tìm kiếm thông tin sinh viên theo tên sinh viên");
                //Console.WriteLine("\t\t2.4 - Tìm kiếm thông tin sinh viên theo khoảng GPA.");
                //Console.WriteLine("\t\t2.5 - Tìm kiếm thông tin sinh viên theo GPA.");
                Console.Write("\tChọn thao tác: ");
                string input = Console.ReadLine();

                if (input == "2.1") TimKiemTheoID();
                else if (input == "2.2") TimKiemTheoMaLop();
                else if (input == "2.3") TimKiemTheoTen();
                else if (input == "2.4")
                {
                    TimKiemTheoKhoangGPA();
                }
                else if (input == "2.5") TimKiemTheoGPA();
                else if (input.ToLower() == "x") break;
                else Console.WriteLine("\t\tLựa chọn không hợp lệ");
                Console.WriteLine("\tPress any key continue...");
                Console.ReadKey();


            }
        }

        //Sắp xếp theo điểm gpa giảm dần
        static void SapXepTheoGpa()
        {
            for(int i = 0; i < count - 1; i++)
            {
                for(int j = 0; j < count - i - 1; j++)
                {
                    if (DSSV[j].gpa < DSSV[j + 1].gpa)
                    {
                        SinhVien tmp = DSSV[j];
                        DSSV[j] = DSSV[j + 1];
                        DSSV[j + 1] = tmp;
                    }
                }
            }
            HienThiDanhSach(count);
        }

        static void SapXepTheoTuoi()
        {
            for(int i = 0; i < count - 1; i++)
            {
                for(int j = 0; j < count - i - 1;j++)
                {
                    if (DSSV[j].age < DSSV[j + 1].age)
                    {
                        SinhVien tmp = DSSV[j];
                        DSSV[j] = DSSV[j + 1];
                        DSSV[j + 1] = tmp;
                    }
                }
            }
            HienThiDanhSach(count);
        }
        
        // Sắp xếp tên của các sinh viên theo bảng chữ cái [A --> Z]
        static void SapXepTheoTen()
        {
            for(int i = 0; i < count; i++)
            {
                for(int j = 0; j < count - i - 1; j++)
                {
                    if (string.Compare(DSSV[j].name, DSSV[j + 1].name, true) == 1) // Tham số thứ 3 sẽ không phân biệt hoa thường
                    {
                        SinhVien tmp = DSSV[j];
                        DSSV[j] = DSSV[j + 1];
                        DSSV[j + 1] = tmp;
                    }
                }
            }
            HienThiDanhSach(count);
        }
        //Menu các thao tác sắp xếp

        static void VeMenuSapXep()
        {
            Console.WriteLine("Chọn thao tác: 3");
            Console.WriteLine("\t[3] - Sắp xếp: ");
            Console.WriteLine("\t\tDanh sách thao tác sắp xếp: ");
            Console.WriteLine("\t\t3.1 - Sắp xếp theo sinh viên GPA.");
            Console.WriteLine("\t\t3.2 - Sắp xếp sinh viên theo tuổi.");
            Console.WriteLine("\t\t3.3 - Sắp xếp sinh viên theo tên.");
            Console.WriteLine("\t\tx - Thoát");
        }
        static void SapXep()
        {
            while (true) {
                Console.Clear();
                VeMenuChinh(current_user.Value.role);
                VeMenuSapXep();
                //Console.WriteLine("\t\tDanh sách thao tác sắp xếp: ");
                //Console.WriteLine("\t\t3.1 - Sắp xếp theo sinh viên GPA.");
                //Console.WriteLine("\t\t3.2 - Sắp xếp sinh viên theo tuổi.");
                //Console.WriteLine("\t\t3.3 - Sắp xếp sinh viên theo tên.");
                Console.Write("\t\tChọn thao tác: ");
                string input = Console.ReadLine();
                if (input == "3.1") SapXepTheoGpa();
                else if (input == "3.2") SapXepTheoTuoi();
                else if (input == "3.3") SapXepTheoTen();
                else if (input.ToLower() == "x")
                {
                    break;
                }
                else Console.WriteLine("\t\t\tLựa chọn không hợp lệ");
                Console.WriteLine("\tPress any key continue...");
                Console.ReadKey();
            }
        }

        // Hàm trả về thông tin sinh viên có gpa lớn nhất
        static SinhVien TimSinhVienTotNhat()
        {
            if(count == 0) return new SinhVien();
            SinhVien svMax = DSSV[0];
            for (int i = 0; i < count; i++)
            {
                if (svMax.gpa < DSSV[i].gpa) svMax = DSSV[i];
            }

           return svMax;
        }

        // Hàm trả về thông tin sinh viên có gpa thấp nhất
        static SinhVien TimSinhVienTeNhat()
        {
            if (count == 0) return new SinhVien();
            SinhVien svMin = DSSV[0];
            for (int i = 0; i < count; i++)
            {
                if (svMin.gpa >  DSSV[i].gpa) svMin = DSSV[i];
            }
            return svMin;
        }

        // Hàm trả về điểm gpa trung bình của tất cả sinh viên
        static double TinhDiemTB()
        {
            if(count == 0) return 0;
            
            double tong_gpa = 0;
            for (int i = 0; i < count; i++)
            {
                tong_gpa += DSSV[i].gpa;

            }
            return tong_gpa / count;
           
        }

        // Hàm tính tỉ lệ khá, giỏi, trung bình, yếu trên toàn hệ thống
        static void TinhTiLeKhaGioi(out double tiLeKha, out double tiLeGioi, out double tileTB, out double tileYeu)
        {
            int soKha = 0, soGioi = 0, soTB = 0, soYeu = 0;

            for (int i = 0; i < count; i++)
            {
                if (DSSV[i].gpa >= 8.0)
                    soGioi++;
                else if (DSSV[i].gpa >= 7.0 && DSSV[i].gpa < 8)
                    soKha++;
                else if (DSSV[i].gpa >= 5.0 && DSSV[i].gpa < 7.0)
                    soTB++;
                else soYeu++;
            }

            tiLeKha = (double)soKha / count * 100;
            tiLeGioi = (double)soGioi / count * 100;
            tileTB = (double)soTB / count * 100;
            tileYeu = (double)soYeu / count * 100;

        }

        // Hàm vẽ cột thống kê tỉ lệ học lực sinh viên theo biểu đồ
        static void InCot(string nhan, double tile)
        {
            int soCot = (int)(tile / 2); // mỗi cột đại diện 2%
            Console.Write($"{nhan}: ");
            for (int i = 0; i < soCot; i++)
            {
                Console.Write("█");
            }
            Console.WriteLine($" ({tile:F2}%)");
        }

        static void ThongKe()
        {
            Console.OutputEncoding = Encoding.UTF8;

            double diemTB = TinhDiemTB();
            double tileKha, tileGioi, tileTB, tileYeu;
            TinhTiLeKhaGioi(out tileKha, out tileGioi, out tileTB, out tileYeu);
            SinhVien svTotNhat = TimSinhVienTotNhat();
            SinhVien svTeNhat = TimSinhVienTeNhat();

        

            Console.WriteLine("\n\t===== THỐNG KÊ LỚP SINH VIÊN =====");
            Console.WriteLine($"\t➤ Điểm trung bình của lớp: {diemTB:F2}");
            Console.WriteLine($"\t➤ Tỉ lệ sinh viên giỏi : {tileGioi:F2}%");
            Console.WriteLine($"\t➤ Tỉ lệ sinh viên khá  : {tileKha:F2}%");
            Console.WriteLine($"\t➤ Tỉ lệ sinh viên trung bình: {tileTB:F2}%");
            Console.WriteLine($"\t➤ Tỉ lệ sinh viên trung bình: {tileYeu:F2}%");

            Console.WriteLine("\n\t➤ Sinh viên có điểm GPA cao nhất:");
            PrintBorderRow();
            Print_Row("Mã SV", "Tên", "Tuổi", "Giới tính", "Lớp", "GPA");
            PrintBorderRow();
            Print_Row(svTotNhat.id, svTotNhat.name, svTotNhat.age.ToString(), svTotNhat.gender, svTotNhat.Class, svTotNhat.gpa.ToString());
            PrintBorderRow();

            Console.WriteLine("\n\t➤ Sinh viên có điểm GPA thấp nhất:");
            PrintBorderRow();
            Print_Row("Mã SV", "Tên", "Tuổi", "Giới tính", "Lớp", "GPA");
            PrintBorderRow();
            Print_Row(svTeNhat.id, svTeNhat.name, svTeNhat.age.ToString(), svTeNhat.gender, svTeNhat.Class, svTeNhat.gpa.ToString());
            PrintBorderRow();

            Console.WriteLine("\n\t\t\t📊 BIỂU ĐỒ TỶ LỆ (mỗi █ = 2%)");
            Console.WriteLine();
           
            InCot("\tGiỏi", tileGioi);

            InCot("\n\tKhá ", tileKha);
     
            InCot("\n\tTB  ", tileTB);
        
            InCot("\n\tYếu ", tileYeu);
        }
      

        // Xuất file PDF
        static void XuatFilePDF()
        {
            Console.Clear();
            Console.WriteLine("==== XUẤT DỮ LIỆU RA FILE PDF ====");

            if (count == 0)
            {
                Console.WriteLine("Không có dữ liệu sinh viên để xuất ra PDF.");
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
                return;
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "DanhSachSinhVien.pdf");

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Document doc = new Document(PageSize.A4, 25, 25, 30, 30); // Lề: trái, phải, trên, dưới
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // Đảm bảo hỗ trợ tiếng Việt (UTF-8) - cần font hỗ trợ tiếng Việt
                    // Để hỗ trợ tiếng Việt, bạn cần nhúng một font có hỗ trợ UTF-8 (ví dụ: Arial Unicode MS, Times New Roman, DejaVuSans)
                    // Hoặc đơn giản hơn, nếu font không quá phức tạp, bạn có thể thử sử dụng BaseFont.IDENTITY_H
                    BaseFont bf = BaseFont.CreateFont("c:\\windows\\fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    Font fontHeader = new Font(bf, 14, Font.BOLD, BaseColor.Blue);
                    Font fontNormal = new Font(bf, 10, Font.NORMAL, BaseColor.Black);
                    Font fontTable = new Font(bf, 9, Font.NORMAL, BaseColor.Black);

                    // Tiêu đề
                    Paragraph title = new Paragraph("DANH SÁCH SINH VIÊN", fontHeader);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 10f;
                    doc.Add(title);

                    // Tạo bảng
                    PdfPTable table = new PdfPTable(6); // 6 cột
                    table.WidthPercentage = 100; // Chiếm 100% chiều rộng trang
                    table.SetWidths(new float[] { 1.5f, 3f, 1f, 1.5f, 1.5f, 1f }); // Tỷ lệ độ rộng các cột

                    // Thêm tiêu đề cột vào bảng
                    AddCellToTable(table, "Mã SV", fontNormal, true);
                    AddCellToTable(table, "Tên", fontNormal, true);
                    AddCellToTable(table, "Tuổi", fontNormal, true);
                    AddCellToTable(table, "Giới tính", fontNormal, true);
                    AddCellToTable(table, "Lớp", fontNormal, true);
                    AddCellToTable(table, "GPA", fontNormal, true);

                    // Thêm dữ liệu sinh viên vào bảng
                    for (int i = 0; i < count; i++)
                    {
                        SinhVien sv = DSSV[i];
                        AddCellToTable(table, sv.id, fontTable, false);
                        AddCellToTable(table, sv.name, fontTable, false);
                        AddCellToTable(table, sv.age.ToString(), fontTable, false);
                        AddCellToTable(table, sv.gender, fontTable, false);
                        AddCellToTable(table, sv.Class, fontTable, false);
                        AddCellToTable(table, sv.gpa.ToString("F2"), fontTable, false);
                    }

                    doc.Add(table);
                    doc.Close();
                }

                Console.WriteLine($"Đã xuất dữ liệu ra file PDF thành công tại: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xuất file PDF: {ex.Message}");
                Console.WriteLine("Đảm bảo bạn có font 'arial.ttf' hoặc một font khác hỗ trợ tiếng Việt trên hệ thống.");
            }
        }

        // Hàm trợ giúp để thêm cell vào bảng PDF
        private static void AddCellToTable(PdfPTable table, string text, Font font, bool isHeader)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5;
            if (isHeader)
            {
                cell.BackgroundColor = new BaseColor(220, 220, 220); // Màu nền xám nhạt cho header
            }
            table.AddCell(cell);
        }

        static void GhiFile()
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string file_path = "D:\\CS\\QuanLySinhVien\\data.txt";
            using (StreamWriter writer = new StreamWriter(file_path, false, Encoding.UTF8))
            {
                for (int i = 0; i < count; i++)
                {
                    SinhVien sv = DSSV[i];
                    writer.WriteLine($"{sv.id};{sv.name};{sv.age};{sv.gender};{sv.Class};{sv.gpa}");
                }
            }
            Console.WriteLine("\t\tĐã lưu thông tin sinh viên vào file thành công");
        }

        static void VeMenuChinh(Role role)
        {
            Console.WriteLine($"Xin chào ({role})");
            Console.WriteLine("Danh sách các thao tác cho " + role + ": ");
            Console.WriteLine("1 - Hiển thị danh sách sinh viên.");
            Console.WriteLine("2 - Tìm kiếm.");
            Console.WriteLine("3 - Sắp xếp.");
            Console.WriteLine("4 - Thống kê.");
            if (role == Role.User)
                Console.WriteLine("5 - Reset mật khẩu.");
            if (role == Role.Admin)
            {
                Console.WriteLine("5 - Thêm sinh viên.");
                Console.WriteLine("6 - Sửa sinh viên.");
                Console.WriteLine("7 - Xóa sinh viên.");
                Console.WriteLine("8 - Sao lưu.");
                Console.WriteLine("9 - Xuất file PDF.");
                Console.WriteLine("10 - Reset mật khẩu cho người dùng.");
            }
            Console.WriteLine("0 - Đăng xuất");
            Console.WriteLine("x - Thoát");
            
        }


        static void MainMenu()
        {
            while (true)
            {
                if (current_user == null)
                {
                    //Console.Clear();
                    Console.WriteLine("CHÀO MỪNG ĐẾN VỚI HỆ THỐNG QUẢN LÝ SINH VIÊN");
                    Console.WriteLine("1 - Đăng nhập");
                    Console.WriteLine("2 - Đăng ký tài khoản");
                    Console.WriteLine("0 - Thoát");
                    Console.Write("Chọn: ");
                    string chon = Console.ReadLine();
                    Console.Clear();

                    if (chon == "1")
                    {
                        Login();
                    }
                    else if(chon == "2")
                    {
                        Register(accounts);
                        SaveAllAccounts();
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else if (chon == "0") break;
                    else { 
                        Console.WriteLine("Lựa chọn không hợp lệ");
                        Console.WriteLine("Press any key continue...");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                else
                {
                    Console.Clear();
                    //Console.WriteLine($"Xin chào ({current_user.Value.role})");
                    //Console.WriteLine($"Danh sách các thao tác cho {current_user.Value.role}: ");
                    //Console.WriteLine("1 - Hiển thị danh sách sinh viên.");
                    //Console.WriteLine("2 - Tìm kiếm.");
                    //Console.WriteLine("3 - Sắp xếp.");
                    //Console.WriteLine("4 - Thống kê.");
                    //if(current_user.Value.role == Role.User)
                    //{
                    //    Console.WriteLine("5 - Reset mật khẩu.");
                    //}

                    //if (current_user.Value.role == Role.Admin)
                    //{
                    //    Console.WriteLine("5 - Thêm thông tin sinh viên.");
                    //    Console.WriteLine("6 - Sửa thông tin sinh viên.");
                    //    Console.WriteLine("7 - Xóa thông tin sinh viên.");
                    //    Console.WriteLine("8 - Sao lưu");
                    //    Console.WriteLine("9 - Xuất dữ liệu ra PDF");
                    //    Console.WriteLine("10 - Reset mật khẩu cho người dùng.");
                    //}

                    //Console.WriteLine("0 - Đăng xuất");
                    //Console.WriteLine("x - Thoát");
                    string choice;
                    VeMenuChinh(current_user.Value.role);
                    Console.Write("Chọn thao tác: ");
                    choice = Console.ReadLine();

                    if (choice == "1") 
                    {
                        Console.WriteLine("\t[1] In danh sách sinh viên: ");
                        HienThiDanhSach(count);
                    }
                    else if (choice == "2")
                    {
                        Console.WriteLine("\t[2] Tìm kiếm sinh viên: ");
                        TimKiem();
                    }
                    else if (choice == "3") 
                    {
                        Console.WriteLine("\t[3] Sắp xếp: ");
                        SapXep();

                    }

                    else if (choice == "4") 
                    {
                        Console.WriteLine("\t[4] Thống kê: ");
                        ThongKe();
                    }
                    else if (choice == "5") 
                    {
                        if (current_user.Value.role == Role.Admin)
                        {
                            Console.WriteLine("\t[5] Thêm sinh viên: ");
                            ThemSinhVien();
                        }
                        else if(current_user.Value.role == Role.User)
                        {
                            ResetPassword_User();
                            SaveAllAccounts();
                        }
                        
                    }

                    else if (choice == "6") 
                    {
                        if (current_user.Value.role == Role.Admin)
                        {
                            SuaThongTin();
                        }
                    }       

                    else if (choice == "7")
                    {
                        Console.WriteLine("\t[7] Xóa thông tin sinh viên: ");
                        XoaThongTin();

                    }   
                    else if (choice == "8")
                    {
                        Console.WriteLine("\t[8] Sao lưu: ");
                        GhiFile();

                    }

                    else if(choice == "9")
                    {
                        if (current_user.Value.role == Role.Admin) XuatFilePDF();
                        else Console.WriteLine("Bạn không có quyền thực hiện chức năng này.");
                        Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                    }

                    else if (choice == "10")
                    {
                        if(current_user.Value.role == Role.Admin)
                        {
                            Console.WriteLine("\t[10] Reset mật khẩu cho người dùng.");
                            Resetpassword_Admin();
                            SaveAllAccounts();
                        }
                    }
                    else if (choice == "0") 
                    {
                        Logout();

                    }
                    else if (choice == "x") 
                    {
                        Console.WriteLine("\t[x] Thoát hệ thống: ");
                        Console.Write("\t\tBạn có muốn thoát hệ thống quản lý sinh viên không (y/n)?: ");
                        string confirm = Console.ReadLine();
                        if (confirm == "y")
                        {
                            Console.WriteLine("\t\tThoát hệ thống thành công.");
                            return;
                        }
                    }
                    else Console.WriteLine("\tLựa chọn không hợp lệ.");

                    Console.WriteLine("Press any key continue...");
                    Console.ReadKey(); // Chờ người dùng nhập thao tác bất kì từ bàn phím để dừng chương trình
                    Console.Clear(); // Xóa hết dữ liệu về thao tác vừa nhập   
                }
                
            }
        }

        static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            LoadAllAccounts(); // load tài khoản
            DocThongTin(); // Load dữ liệu từ file
            MainMenu();
        }
    }
}
