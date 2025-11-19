using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RBACSystem.BusinessLogic.RBAC;
using RBACSystem.Models.RBAC;
using System.Data;

public partial class Pages_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            LoadUsers(string.Format("این تستی برای متد است: {0}","منم"));
    }
    public void LoadUsers(string a)
    {
        lbltest.Text = a;
        var userManager = new UserManager();
        List<User> users = userManager.GetAllUsers(true);

        repListUsers.DataSource = users;
        repListUsers.DataBind();
    }

    protected void btnDeleteUser_Click(object sender, EventArgs e)
    {
        Button btnChangeStatus = (Button)sender;
        int userId = Int32.Parse(btnChangeStatus.CommandArgument);
        bool isActive = bool.Parse(btnChangeStatus.CommandName);

        var userManager = new UserManager();

        userManager.ToggleUserStatus(userId,!isActive, 1); // Second Parameter is Session Curent User ID        
        LoadUsers(string.Format("این تستی برای متد است {0}", "منم"));
            
    }

}