using Npgsql;
using VehicleSystem.models;
//using VehicleSystem.Models;

namespace VehicleSystem.Data
{
    public class TechnicalIncomeHelper
    {
        // Fiiro gaar ah: kani waa PostgreSQL connection string (Npgsql format),
        // ma aha SQL Server format. Talo: geli password-ka config/env variable
        // halkii aad ugu qori lahayd si toos ah code-ka dhexdiisa.
        string connectionString = "Host=aws-0-eu-west-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres;Password=Cade112345##779";

        NpgsqlConnection con;
        NpgsqlDataReader dr;
        NpgsqlCommand cmd;
        string query;

        // ---------- Add Technical Income ----------
        public Response AddTechnicalIncome(TechnicalIncome t)
        {
            try
            {
                using (con = new NpgsqlConnection(connectionString))
                {
                    con.Open();

                    query = @"INSERT INTO technical_income (vehicle_type, month, amount, description, created_at)
                              VALUES (@vehicleType, @month, @amount, @description, NOW())";

                    cmd = new NpgsqlCommand(query, con);
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
                using (con = new NpgsqlConnection(connectionString))
                {
                    con.Open();

                    query = @"UPDATE technical_income
                              SET vehicle_type = @vehicleType,
                                  month = @month,
                                  amount = @amount,
                                  description = @description
                              WHERE id = @id";

                    cmd = new NpgsqlCommand(query, con);
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

                using (con = new NpgsqlConnection(connectionString))
                {
                    con.Open();

                    query = "SELECT * FROM technical_income WHERE 1 = 1";

                    if (id != 0) query += " AND id = @id";
                    if (!string.IsNullOrEmpty(vehicleType)) query += " AND vehicle_type = @vehicleType";

                    query += " ORDER BY month DESC";

                    cmd = new NpgsqlCommand(query, con);
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
                using (con = new NpgsqlConnection(connectionString))
                {
                    con.Open();

                    query = @"SELECT EXTRACT(YEAR FROM month)::int AS yr,
                                     EXTRACT(MONTH FROM month)::int AS mo,
                                     SUM(amount) AS total_amount
                              FROM technical_income
                              GROUP BY EXTRACT(YEAR FROM month), EXTRACT(MONTH FROM month)
                              ORDER BY yr, mo";

                    cmd = new NpgsqlCommand(query, con);
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
                using (con = new NpgsqlConnection(connectionString))
                {
                    con.Open();

                    query = "DELETE FROM technical_income WHERE id = @id";
                    cmd = new NpgsqlCommand(query, con);
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