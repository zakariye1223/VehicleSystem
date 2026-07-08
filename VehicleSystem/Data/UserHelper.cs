using Npgsql;
using System.Security.Cryptography;
using VehicleSystem.models;
//using VehicleSystem.Models;

namespace VehicleSystem.Data
{
    public class UserHelper
    {
        // Fiiro gaar ah: kani waa PostgreSQL connection string (Npgsql format).
        // Talo: geli password-ka config/env variable halkii aad ugu qori
        // lahayd si toos ah code-ka dhexdiisa.
        string connectionString = "Host=aws-0-eu-west-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.jmpnskijsubvpvukiyzd;Password=Cade112345##779";

        NpgsqlConnection con;
        NpgsqlDataReader dr;
        NpgsqlCommand cmd;
        string query;

        // ---------- Password Hashing Helpers ----------
        // Waxaan isticmaalaynaa PBKDF2 (Rfc2898DeriveBytes) - salt gaar ah + hash, labaduba waxaa la kaydiyaa hal string.
        private string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password: System.Text.Encoding.UTF8.GetBytes(password),
                salt: salt,
                iterations: 100_000,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: 32);

            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

            byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                password: System.Text.Encoding.UTF8.GetBytes(password),
                salt: salt,
                iterations: 100_000,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: 32);

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
        }

        // ---------- Register ----------
        public Response RegisterUser(User u)
        {
            try
            {
                using (con = new NpgsqlConnection(connectionString))
                {
                    con.Open();

                    // Hubi in username-ka horeba loo isticmaalin
                    query = "SELECT COUNT(1) FROM users WHERE username = @username";
                    cmd = new NpgsqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", u.Username);
                    long exists = (long)cmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        return new Response { Status = false, Message = "Username-kan horeba waa la isticmaalay" };
                    }

                    string hashedPassword = HashPassword(u.Password);

                    query = "INSERT INTO users (username, password_hash, created_at) VALUES (@username, @passwordHash, NOW())";
                    cmd = new NpgsqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", u.Username);
                    cmd.Parameters.AddWithValue("@passwordHash", hashedPassword);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return new Response { Status = true, Message = "User Registered Successfully" };
                    }
                    else
                    {
                        return new Response { Status = false, Message = "User Registration Failed" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- Get Users (list all, or by id) ----------
        // Fiiro: password_hash lama soo celiyo amni darteed
        public Response GetUsers(int id = 0)
        {
            try
            {
                List<User> data = new List<User>();

                using (con = new NpgsqlConnection(connectionString))
                {
                    con.Open();

                    query = "SELECT id, username, created_at FROM users";
                    if (id != 0) query += " WHERE id = @id";
                    query += " ORDER BY id DESC";

                    cmd = new NpgsqlCommand(query, con);
                    if (id != 0) cmd.Parameters.AddWithValue("@id", id);

                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            data.Add(new User()
                            {
                                Id = Convert.ToInt32(dr["id"]),
                                Username = dr["username"].ToString(),
                                CreatedAt = Convert.ToDateTime(dr["created_at"])
                            });
                        }

                        return new Response { Status = true, Message = "Users found", Data = data };
                    }
                    else
                    {
                        return new Response { Status = false, Message = "No users found" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- Update User (username and/or password) ----------
        public Response UpdateUser(User u)
        {
            try
            {
                using (con = new NpgsqlConnection(connectionString))
                {
                    con.Open();

                    if (!string.IsNullOrEmpty(u.Password))
                    {
                        // Password cusub ayaa la geliyay - hash-gareey oo labadaba beddel
                        string hashedPassword = HashPassword(u.Password);

                        query = "UPDATE users SET username = @username, password_hash = @passwordHash WHERE id = @id";
                        cmd = new NpgsqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@username", u.Username);
                        cmd.Parameters.AddWithValue("@passwordHash", hashedPassword);
                        cmd.Parameters.AddWithValue("@id", u.Id);
                    }
                    else
                    {
                        // Password lama gelin - kaliya username ayaa la beddelayaa
                        query = "UPDATE users SET username = @username WHERE id = @id";
                        cmd = new NpgsqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@username", u.Username);
                        cmd.Parameters.AddWithValue("@id", u.Id);
                    }

                    if (cmd.ExecuteNonQuery() > 0)
                        return new Response { Status = true, Message = "User Updated Successfully" };
                    else
                        return new Response { Status = false, Message = "User Update Failed" };
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- Delete User ----------
        public Response DeleteUser(int id)
        {
            try
            {
                using (con = new NpgsqlConnection(connectionString))
                {
                    con.Open();

                    query = "DELETE FROM users WHERE id = @id";
                    cmd = new NpgsqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", id);

                    if (cmd.ExecuteNonQuery() > 0)
                        return new Response { Status = true, Message = "User deleted successfully" };
                    else
                        return new Response { Status = false, Message = "User deletion failed" };
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }

        // ---------- Login ----------
        public Response LoginUser(User u)
        {
            try
            {
                using (con = new NpgsqlConnection(connectionString))
                {
                    con.Open();

                    query = "SELECT id, username, password_hash, created_at FROM users WHERE username = @username";
                    cmd = new NpgsqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", u.Username);

                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        dr.Read();
                        string storedHash = dr["password_hash"].ToString();

                        if (VerifyPassword(u.Password, storedHash))
                        {
                            var loggedInUser = new User
                            {
                                Id = Convert.ToInt32(dr["id"]),
                                Username = dr["username"].ToString(),
                                CreatedAt = Convert.ToDateTime(dr["created_at"])
                            };

                            return new Response { Status = true, Message = "Login Successful", Data = loggedInUser };
                        }
                        else
                        {
                            return new Response { Status = false, Message = "Password khalad ah" };
                        }
                    }
                    else
                    {
                        return new Response { Status = false, Message = "User lama helin" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { Status = false, Message = ex.Message };
            }
        }
    }
}