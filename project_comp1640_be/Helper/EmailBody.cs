namespace project_comp1640_be.Helper
{
    public class EmailBody
    {
        public static string EmailStringBody(string email, string emailToken)
        {
            return $@"<html>
                  <head></head>
                  <body style=""margin:0; padding: 0;font-family: Arial, Helvetica, sans-serif;"">
                    <div style=""height:auto;background: linear-gradient(to top, #c9c9ff 50%, #6e6ef6 90%) no-repeat;width: 400px;padding: 30px;"">
                      <div>
                        <div>
                          <h1>Reset Your Password</h1>
                          <hr>
                          <p>You're receiving this e-mail because you requested a password reset for your Annual University Magazine account.</p>
                          <p>Please tap the button below to choose a new password.</p>
                          <a href=""http://localhost:4200/Reset-Password?email={email}&code={emailToken}"" target="""" _blank"" style=""background:#0d6efd;padding:10px;border:none;
                            color:white;border-radius:4px;display:block;margin:0 auto;width:50%;text-align:center;text-decoration:none"">Reset Password</a><br>
            
                            <p>Kind Regards,<br><br>
                            Annual University Magazine </P>
                        </div>
                      </div>
                    </div>

                  </body>
                </html>";
        }


        public static string AddNewArticleEmailStringBody(int id)
        {
            return $@"<html>
                        <head></head>
                        <body style=""margin:0; padding: 0;font-family: Arial, Helvetica, sans-serif;"">
                            <div
                                style=""height:auto;background: linear-gradient(to top, #c9c9ff 50%, #6e6ef6 90%) no-repeat;width: 400px;padding: 30px;"">
                                <div>
                                    <div>
                                        <h1>The new article posted</h1>
                                        <hr>
                                        <p>You received this email because an article in your faculty was posted by your faculty member.</p>
                                        <p>Please click the button below to view the new article.</p>
                                        <a href=""http://localhost:4200/Detail-Articles/{id}"" target="""" _blank
                                            style=""background:#0d6efd;padding:10px;border:none;
                                                        color:white;border-radius:4px;display:block;margin:0 auto;width:50%;text-align:center;text-decoration:none"">View article</a><br>
                                        <p>Kind Regards,<br><br>
                                            Annual University Magazine</P>
                                    </div>
                                </div>
                            </div>
                        </body>
                        </html>";
        }
    }
}
