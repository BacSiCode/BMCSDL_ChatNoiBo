# Hệ thống Chat Nội Bộ Có Tính Năng Bảo Mật

## Giới thiệu
Đây là đồ án học phần Cơ sở dữ liệu với mục tiêu xây dựng một ứng dụng chat nội bộ phục vụ trao đổi thông tin trong mạng nội bộ tổ chức/doanh nghiệp, đồng thời tích hợp cơ chế phân quyền và bảo mật dữ liệu.

Dự án giúp nhóm thực hành thiết kế database, lập trình ứng dụng desktop và triển khai các cơ chế bảo mật trong Oracle.

---

## Bài toán đặt ra
Các doanh nghiệp thường sử dụng ứng dụng công cộng (Messenger, Zalo, …) để trao đổi công việc, dẫn đến:
- Rò rỉ thông tin nội bộ
- Thông tin phân tán, khó kiểm soát
- Không đồng bộ phòng ban
- Khó gửi thông báo toàn công ty
- 
→ Cần một hệ thống chat nội bộ có quản lý người dùng và bảo mật.

---

## Mục tiêu
- Xây dựng hệ thống chat nội bộ trong mạng LAN
- Thiết kế cơ sở dữ liệu quản lý người dùng và tin nhắn
- Phân quyền theo vai trò (Admin / User / Leader)
- Bảo mật mật khẩu và nội dung tin nhắn
- Thực hành triển khai và kiểm thử hệ thống

---

## Công nghệ sử dụng
- C# WinForms (.NET)
- Oracle Database
- SQL
- Visual Studio

---

## Phân tích nghiệp vụ
Hệ thống cần:
- Đăng ký / Đăng nhập
- Chat 1-1
- Chat nhóm / phòng ban
- Chat toàn công ty
- Phân quyền người dùng
- Giám sát và quản lý tài khoản
- Bảo mật mật khẩu và dữ liệu

---

## Thiết kế cơ sở dữ liệu

Các bảng chính:
- TAIKHOAN (quản lý người dùng)
- NHOMCHAT (quản lý nhóm)
- THANHVIENNHOM (thành viên nhóm)
- TINNHAN (tin nhắn)
- BANBE (danh sách bạn bè)
- PHONGBAN (phòng ban)
- THONGBAO (thông báo hệ thống)

Thực hiện:
- Thiết kế ERD
- Chuẩn hóa dữ liệu
- Viết truy vấn CRUD

---

## Phân quyền người dùng

### SupAdmin
- Quản lý toàn hệ thống

### Admin/Leader
- Quản lý user thuộc phòng ban
- Duyệt tài khoản mới
- Tạo và quản lý nhóm chat

### User
- Đăng nhập
- Gửi/nhận tin nhắn
- Chỉ xem dữ liệu phòng ban của mình

---

## Bảo mật
- Mã hóa mật khẩu người dùng
- Mã hóa dữ liệu tin nhắn (mức cơ bản)
- Áp dụng cơ chế phân quyền database:
  - Role / Profile
  - Tablespace
  - FGA
  - VPD
  - OLS

Giúp hạn chế truy cập trái phép và bảo vệ dữ liệu nội bộ.

---

## Kịch bản demo (Scenario)

### Bước 1 – Cài đặt
- Cài Oracle Database
- Import database
- Chạy project bằng Visual Studio

### Bước 2 – Đăng ký/Đăng nhập
- Tạo tài khoản
- Admin xác nhận
- Đăng nhập hệ thống

### Bước 3 – Chat
- Tham gia phòng ban
- Chat 1-1 hoặc nhóm
- Tin nhắn lưu vào database

### Bước 4 – Quản lý
- Admin quản lý user
- Gửi thông báo
- Phân quyền tài khoản

### Bước 5 – Kiểm thử
- Kiểm tra lưu trữ dữ liệu
- Kiểm tra phân quyền
- Kiểm tra bảo mật

---

## Vai trò cá nhân
- Nhóm trưởng
- Thiết kế cơ sở dữ liệu
- Lập trình chức năng đăng nhập/nhắn tin
- Kết nối C# với Oracle
- Kiểm thử và demo hệ thống

---

## Kiến thức học được
- Thiết kế database thực tế
- Lập trình C# WinForms
- Kết nối Oracle bằng SQL
- Hiểu mô hình Client–Server
- Thực hành phân quyền và bảo mật dữ liệu
- Làm việc nhóm và quản lý tiến độ

---

