using Core;

namespace Client;

public partial class ChatRoom : Form
{
    public ChatRoom()
    {
        InitializeComponent();
        listBox_user.Items.Add(Singleton.Instance.Nickname);
        Singleton.Instance.UserEnterResponsed += UserEnterResponsed;
        Singleton.Instance.UserLeaveResponsed += UserLeaveResponsed;
        Singleton.Instance.ChatResponsed += ChatResponsed;

        FormClosing += async (s, e) =>
        {
            Singleton.Instance.UserEnterResponsed -= UserEnterResponsed;
            Singleton.Instance.UserLeaveResponsed -= UserLeaveResponsed;
            Singleton.Instance.ChatResponsed -= ChatResponsed;
            UserLeavePacket packet = new UserLeavePacket(Singleton.Instance.Nickname);
            await Singleton.Instance.Socket.SendAsync(packet.Serialize(), System.Net.Sockets.SocketFlags.None);
        };
    }

    // 메시지 전송하기
    private async void btn_send_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(tbx_msg.Text))
        {
            return;
        }

        ChatPacket packet = new ChatPacket(Singleton.Instance.Nickname, tbx_msg.Text);
        await Singleton.Instance.Socket.SendAsync(packet.Serialize(), System.Net.Sockets.SocketFlags.None);
        tbx_msg.Text = null;
    }

    // 맨 아래로 스크롤 내리기
    private void ScrollToDown()
    {
        listBox_msg.SelectedIndex = listBox_msg.Items.Count - 1;
        listBox_msg.SelectedIndex = -1;
    }

    private void UserEnterResponsed(object? sender, EventArgs e)
    {
        UserEnterPacket packet = (UserEnterPacket)sender!;
        listBox_user.Items.Add(packet.Nickname);
    }

    private void UserLeaveResponsed(object? sender, EventArgs e)
    {
        UserLeavePacket packet = (UserLeavePacket)sender!;
        listBox_user.Items.Remove(packet.Nickname);
    }

    private void ChatResponsed(object? sender, EventArgs e)
    {
        ChatPacket packet = (ChatPacket)sender!;
        listBox_msg.Items.Add($"{packet.Nickname} : {packet.Message}");
        ScrollToDown();
    }
}
