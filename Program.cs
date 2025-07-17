using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

public class Create_date
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("conneting to database...");


        string connectionString = "localhost_server";

        var service = new CustomerService(connectionString);


        DateTime startDate = new DateTime(2024, 1, 1); //วันเริ่มต้นในการใช้งาน
        DateTime endDate = new DateTime(2024, 12, 31); //วันสิ้นสุดในการใช้งาน
        DataTable result = await service.GetCustomerInfoAsync("CUST001", startDate, endDate); //ส่งข้อมูลลูกค้าไปบังฟังชั่น GetCustomerInfo โดยส่งค่าพารามิเตอร์ไป 3 ตัว คือ รหัสลูกค้า วันเริ่มต้อนการใช้งาน วันสุดท้ายในการใช้งาน และรอส่งค่ากลับโดยผ่านฟังชั่น async
        DisplayTable(result);
    }

    //DataTable
    private static void DisplayTable(DataTable dt)
    {
        //ถ้าไม่มีข้อมูลให้ขึ้นว่า Data not Found
        if (dt.Rows.Count == 0)
        {
            Console.WriteLine("Data not Found");
            return;
        }

        //แสดงข้อมูลทั้งหมดที่อยู่ใน DataTable
        foreach (DataRow row in dt.Rows)
        {
            foreach (DataColumn col in dt.Columns)
            {
                Console.Write($"{col.ColumnName}: {row[col]}  ");
            }
            Console.WriteLine();
        }
    }
}

//คลาสที่สำหรับติดต่อฐานข้อมูล
public class CustomerService
{
    private readonly string _connectionString;

    public CustomerService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<DataTable> GetCustomerInfoAsync(string id, DateTime startDate, DateTime endDate) //ฟังชั่นที่ใช้สำหรับอ่านข้อมูลลูกค้าที่ส่งมาละคืนค่ากลับไป
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var sql = @"SELECT * FROM Customer WHERE id = @Id AND created_at >= @StartDate AND created_at <= @EndDate";//รันคำสั่ง sql สำหรับ query ข้อมูลใน colum Customer

        //ใช้ anonymous object สำหรับส่ง parameter เข้า SQL
        var parameters = new
        {
            Id = id,
            StartDate = startDate,
            EndDate = endDate
        };

        var reader = await conn.ExecuteReaderAsync(sql, parameters);//ดึงข้อมูลจาก DB แล้วโหลดลง DataTable

        var dt = new DataTable();//สร้าง DataTable
        dt.Load(reader);//โหลดข้อมูลจาก reader ลงใน DataTable
        return dt;//ส่ง DataTable กลับไปยังผู้เรียก
    }
}



/////////////////////////////////////////////////////////////////////////////
/// ฟังชั่นเก่า
/// 
/// 
/// Function GetCustomerInfo(id)
//     Create empty table called resultTable

//     Connect to database using connection string

//     Open database connection

//     Create SQL query:
//         "SELECT * FROM Customer WHERE id = '" + id + "'"

//     Create data adapter using SQL query and connection

//     Fill resultTable with data from data adapter

//     Close connection (automatically via using block)

//     Return resultTable
// End Function

