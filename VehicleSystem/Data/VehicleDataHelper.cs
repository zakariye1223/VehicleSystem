using Microsoft.Data.SqlClient;
using VehicleSystem.models;
//using VehicleSystem.Models;

namespace VehicleSystem.Data
{
    public class VehicleDataHelper
    {
        string connectionString = "Data Source=DESKTOP-16ANKO9\\SQLEXPRESS;Initial Catalog=VehicleManagement;Integrated Security=True;Trust Server Certificate=True";


        SqlConnection con;
        SqlDataReader dr;
        SqlCommand cmd;
        string query;

        // ---------- Add Vehicle Data ----------
        public Response AddVehicleData(VehicleData v)
        {
            try
            {
                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = @"INSERT INTO Vehicle_Data (user_id, vehicle_type, entry_date, amount, description, created_at)
                              VALUES (@userId, @vehicleType, @entryDate, @amount, @description, GETDATE())";

                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@userId", v.UserId);
                    cmd.Parameters.AddWithValue("@vehicleType", v.VehicleType);
                    cmd.Parameters.AddWithValue("@entryDate", v.EntryDate);
                    cmd.Parameters.AddWithValue("@amount", v.Amount);
                    cmd.Parameters.AddWithValue("@description", (object)v.Description ?? DBNull.Value);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return new Response { Status = true, Message = "Vehicle Data Added Successfully" };
                    }
                    else
                    {
                        return new Response { Status = false, Message = "Vehicle Data Insert Failed" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- Update Vehicle Data ----------
        public Response UpdateVehicleData(VehicleData v)
        {
            try
            {
                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = @"UPDATE Vehicle_Data
                              SET vehicle_type = @vehicleType,
                                  entry_date   = @entryDate,
                                  amount       = @amount,
                                  description  = @description
                              WHERE id = @id";

                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@vehicleType", v.VehicleType);
                    cmd.Parameters.AddWithValue("@entryDate", v.EntryDate);
                    cmd.Parameters.AddWithValue("@amount", v.Amount);
                    cmd.Parameters.AddWithValue("@description", (object)v.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", v.Id);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return new Response { Status = true, Message = "Vehicle Data Updated Successfully" };
                    }
                    else
                    {
                        return new Response { Status = false, Message = "Vehicle Data Update Failed" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- Get All / Get by Id / Filter by Vehicle Type ----------
        public Response GetVehicleData(int id = 0, int userId = 0, string vehicleType = null)
        {
            try
            {
                List<VehicleData> data = new List<VehicleData>();

                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = "SELECT * FROM Vehicle_Data WHERE 1 = 1";

                    if (id != 0) query += " AND id = @id";
                    if (userId != 0) query += " AND user_id = @userId";
                    if (!string.IsNullOrEmpty(vehicleType)) query += " AND vehicle_type = @vehicleType";

                    query += " ORDER BY entry_date DESC";

                    cmd = new SqlCommand(query, con);
                    if (id != 0) cmd.Parameters.AddWithValue("@id", id);
                    if (userId != 0) cmd.Parameters.AddWithValue("@userId", userId);
                    if (!string.IsNullOrEmpty(vehicleType)) cmd.Parameters.AddWithValue("@vehicleType", vehicleType);

                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            data.Add(new VehicleData()
                            {
                                Id = Convert.ToInt32(dr["id"]),
                                UserId = Convert.ToInt32(dr["user_id"]),
                                VehicleType = dr["vehicle_type"].ToString(),
                                EntryDate = Convert.ToDateTime(dr["entry_date"]),
                                Amount = Convert.ToDecimal(dr["amount"]),
                                Description = dr["description"] == DBNull.Value ? null : dr["description"].ToString(),
                                CreatedAt = Convert.ToDateTime(dr["created_at"])
                            });
                        }

                        return new Response { Status = true, Message = "Vehicle data found", Data = data };
                    }
                    else
                    {
                        return new Response { Status = false, Message = "No vehicle data found" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- Search & Filter (by date range + vehicle type + keyword) ----------
        public Response SearchVehicleData(string vehicleType, DateTime? fromDate, DateTime? toDate, string keyword)
        {
            try
            {
                List<VehicleData> data = new List<VehicleData>();

                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = "SELECT * FROM Vehicle_Data WHERE 1 = 1";

                    if (!string.IsNullOrEmpty(vehicleType)) query += " AND vehicle_type = @vehicleType";
                    if (fromDate.HasValue) query += " AND entry_date >= @fromDate";
                    if (toDate.HasValue) query += " AND entry_date <= @toDate";
                    if (!string.IsNullOrEmpty(keyword)) query += " AND description LIKE @keyword";

                    query += " ORDER BY entry_date DESC";

                    cmd = new SqlCommand(query, con);
                    if (!string.IsNullOrEmpty(vehicleType)) cmd.Parameters.AddWithValue("@vehicleType", vehicleType);
                    if (fromDate.HasValue) cmd.Parameters.AddWithValue("@fromDate", fromDate.Value);
                    if (toDate.HasValue) cmd.Parameters.AddWithValue("@toDate", toDate.Value);
                    if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            data.Add(new VehicleData()
                            {
                                Id = Convert.ToInt32(dr["id"]),
                                UserId = Convert.ToInt32(dr["user_id"]),
                                VehicleType = dr["vehicle_type"].ToString(),
                                EntryDate = Convert.ToDateTime(dr["entry_date"]),
                                Amount = Convert.ToDecimal(dr["amount"]),
                                Description = dr["description"] == DBNull.Value ? null : dr["description"].ToString(),
                                CreatedAt = Convert.ToDateTime(dr["created_at"])
                            });
                        }

                        return new Response { Status = true, Message = "Vehicle data found", Data = data };
                    }
                    else
                    {
                        return new Response { Status = false, Message = "No matching vehicle data found" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- Delete ----------
        public Response DeleteVehicleData(int id)
        {
            try
            {
                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = "DELETE FROM Vehicle_Data WHERE id = @id";
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", id);

                    if (cmd.ExecuteNonQuery() > 0)
                        return new Response { Status = true, Message = "Vehicle data deleted successfully" };
                    else
                        return new Response { Status = false, Message = "Vehicle data deletion failed" };
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- All Monthly Totals (across all vehicle types combined) ----------
        // Loo isticmaalo Dashboard-ka si loo xisaabiyo bisha ugu lacagta badan
        public Response GetAllMonthlyTotals()
        {
            try
            {
                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = @"SELECT YEAR(entry_date) AS yr, MONTH(entry_date) AS mo, SUM(amount) AS total_amount
                              FROM Vehicle_Data
                              GROUP BY YEAR(entry_date), MONTH(entry_date)
                              ORDER BY yr, mo";

                    cmd = new SqlCommand(query, con);
                    dr = cmd.ExecuteReader();

                    var results = new List<object>();
                    while (dr.Read())
                    {
                        results.Add(new
                        {
                            Year = Convert.ToInt32(dr["yr"]),
                            Month = Convert.ToInt32(dr["mo"]),
                            TotalAmount = Convert.ToDecimal(dr["total_amount"])
                        });
                    }

                    return new Response { Status = true, Message = "Monthly totals retrieved", Data = results };
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- Monthly Report (per vehicle type) ----------
        public Response GetMonthlyReport(int year, int month)
        {
            try
            {
                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = @"SELECT vehicle_type, SUM(amount) AS total_amount, COUNT(*) AS total_entries
                              FROM Vehicle_Data
                              WHERE YEAR(entry_date) = @year AND MONTH(entry_date) = @month
                              GROUP BY vehicle_type";

                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@month", month);

                    dr = cmd.ExecuteReader();

                    var report = new List<object>();
                    while (dr.Read())
                    {
                        report.Add(new
                        {
                            VehicleType = dr["vehicle_type"].ToString(),
                            TotalAmount = Convert.ToDecimal(dr["total_amount"]),
                            TotalEntries = Convert.ToInt32(dr["total_entries"])
                        });
                    }

                    return new Response { Status = true, Message = "Monthly report generated", Data = report };
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }
    }
}
