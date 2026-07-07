using Microsoft.Data.SqlClient;
using VehicleSystem.models;
//using VehicleSystem.Models;

namespace VehicleSystem.Data
{
    public class TechnicalIncomeHelper
    {
        string connectionString = "Data Source=DESKTOP-16ANKO9\\SQLEXPRESS;Initial Catalog=VehicleManagement;Integrated Security=True;Trust Server Certificate=True";


        SqlConnection con;
        SqlDataReader dr;
        SqlCommand cmd;
        string query;

        // ---------- Add Technical Income ----------
        public Response AddTechnicalIncome(TechnicalIncome t)
        {
            try
            {
                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = @"INSERT INTO Technical_Income (vehicle_type, month, amount, description, created_at)
                              VALUES (@vehicleType, @month, @amount, @description, GETDATE())";

                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@vehicleType", t.VehicleType);
                    cmd.Parameters.AddWithValue("@month", t.Month);
                    cmd.Parameters.AddWithValue("@amount", t.Amount);
                    cmd.Parameters.AddWithValue("@description", (object)t.Description ?? DBNull.Value);

                    if (cmd.ExecuteNonQuery() > 0)
                        return new Response { Status = true, Message = "Technical Income Added Successfully" };
                    else
                        return new Response { Status = false, Message = "Technical Income Insert Failed" };
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- Update Technical Income ----------
        public Response UpdateTechnicalIncome(TechnicalIncome t)
        {
            try
            {
                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = @"UPDATE Technical_Income
                              SET vehicle_type = @vehicleType,
                                  month = @month,
                                  amount = @amount,
                                  description = @description
                              WHERE id = @id";

                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@vehicleType", t.VehicleType);
                    cmd.Parameters.AddWithValue("@month", t.Month);
                    cmd.Parameters.AddWithValue("@amount", t.Amount);
                    cmd.Parameters.AddWithValue("@description", (object)t.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", t.Id);

                    if (cmd.ExecuteNonQuery() > 0)
                        return new Response { Status = true, Message = "Technical Income Updated Successfully" };
                    else
                        return new Response { Status = false, Message = "Technical Income Update Failed" };
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- Get All / Get by Id / Filter by Vehicle Type ----------
        public Response GetTechnicalIncome(int id = 0, string vehicleType = null)
        {
            try
            {
                List<TechnicalIncome> data = new List<TechnicalIncome>();

                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = "SELECT * FROM Technical_Income WHERE 1 = 1";

                    if (id != 0) query += " AND id = @id";
                    if (!string.IsNullOrEmpty(vehicleType)) query += " AND vehicle_type = @vehicleType";

                    query += " ORDER BY month DESC";

                    cmd = new SqlCommand(query, con);
                    if (id != 0) cmd.Parameters.AddWithValue("@id", id);
                    if (!string.IsNullOrEmpty(vehicleType)) cmd.Parameters.AddWithValue("@vehicleType", vehicleType);

                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            data.Add(new TechnicalIncome()
                            {
                                Id = Convert.ToInt32(dr["id"]),
                                VehicleType = dr["vehicle_type"].ToString(),
                                Month = Convert.ToDateTime(dr["month"]),
                                Amount = Convert.ToDecimal(dr["amount"]),
                                Description = dr["description"] == DBNull.Value ? null : dr["description"].ToString(),
                                CreatedAt = Convert.ToDateTime(dr["created_at"])
                            });
                        }

                        return new Response { Status = true, Message = "Technical income data found", Data = data };
                    }
                    else
                    {
                        return new Response { Status = false, Message = "No technical income data found" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- All Monthly Totals (across all vehicle types combined) ----------
        // Loo isticmaalo Dashboard-ka si loo xisaabiyo bisha dakhliga farsamada ugu badan
        public Response GetAllMonthlyTotals()
        {
            try
            {
                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = @"SELECT YEAR(month) AS yr, MONTH(month) AS mo, SUM(amount) AS total_amount
                              FROM Technical_Income
                              GROUP BY YEAR(month), MONTH(month)
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

        // ---------- Delete ----------
        public Response DeleteTechnicalIncome(int id)
        {
            try
            {
                using (con = new SqlConnection(connectionString))
                {
                    con.Open();

                    query = "DELETE FROM Technical_Income WHERE id = @id";
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", id);

                    if (cmd.ExecuteNonQuery() > 0)
                        return new Response { Status = true, Message = "Technical income deleted successfully" };
                    else
                        return new Response { Status = false, Message = "Technical income deletion failed" };
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }
    }
}
