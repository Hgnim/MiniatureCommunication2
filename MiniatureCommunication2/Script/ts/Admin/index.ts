import '../../../Style/scss/Admin/index.scss';
import $ from 'jquery';
export function init() {  
    (window as any).createInviteCode_click = createInviteCode_click;
    (window as any).getInviteCode_click = getInviteCode_click;
}
function createInviteCode_click() {
    $.ajax({
        url: theUrlRoot + '/Admin/CreateInviteCode',
        type: "GET",
        contentType: "application/json",
        success: function (response) {
            getInviteCode_click(response);
        },
    });
    
}
function getInviteCode_click(value: string | null = null) {
    function write(v:string) {
        document.getElementById("inviteCodeShow").textContent = v;
    }
    if (value == null) {
        $.ajax({
            url: theUrlRoot + '/Admin/GetInviteCode',
            type: "GET",
            contentType: "application/json",
            success: function (response) {
                write(response);
            },
        });
    } else
        write(value);
}