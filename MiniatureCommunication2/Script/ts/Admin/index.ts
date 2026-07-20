/// <reference path="../../@types/Admin/index.d.ts" />

import '../../../Style/scss/Admin/index.scss';
import $ from 'jquery';
import type InviteCode from '../../types/MC2/Database/InviteCode.type';
import type IdentityUserMini from '../../types/MC2/Database/IdentityUserMini.type';
export function init() {  
    (window as any).createInviteCode_click = createInviteCode_click;
    (window as any).getInviteCode_click = getInviteCode_click;
    (window as any).clearInviteCode_click = clearInviteCode_click;

    (window as any).getUserList_click = getUserList_click;
    (window as any).lockUser_click = lockUser_click;
    (window as any).unlockUser_click = unlockUser_click;
    (window as any).deleteUser_click = deleteUser_click;
}
function createInviteCode_click() {
    $.ajax({
        url: urlPathBase + '/Admin/CreateInviteCode',
        type: "GET",
        contentType: "application/json",
        success: function (response) {
            getInviteCode_click(response);
        },
    });
    
}
function getInviteCode_click(value: InviteCode[] | null = null) {
    function write(v: InviteCode[]) {
        let output = '邀请码 | 是否已被使用 | 过期时间 | 身份';
        v.forEach(ic => {
            output += '\n';
            output += `${ic.code} | ${ic.used ? '是' : '否'} | ${ic.expireAt} | ${ic.role}`;
        });
        document.getElementById("inviteCodeList").textContent = output;
    }
    if (value == null) {
        $.ajax({
            url: urlPathBase + '/Admin/GetInviteCode',
            type: "GET",
            contentType: "application/json",
            success: function (response) {
                write(response);
            },
        });
    } else
        write(value);
}
function clearInviteCode_click() {
    $.ajax({
        url: urlPathBase + '/Admin/ClearInviteCode',
        type: "GET",
        contentType: "application/json",
        success: function (response) {
            getInviteCode_click(response);
        },
    });
}

function getUserList_click() {
    function write(v:IdentityUserMini[]) {
        let output = 'ID | 用户名 | 锁定结束时间';
        v.forEach(ium => {
            output += '\n';
            output += `${ium.id} | ${ium.userName} | ${ium.lockoutEnd}`;
        });
        document.getElementById('userList').textContent = output;
    }
    $.ajax({
        url: urlPathBase + '/Admin/GetUserList',
        type: "GET",
        contentType: "application/json",
        success: function (response) {
            write(response);
        },
    });
}
function lockUser_click() {
    $.ajax({
        url: urlPathBase + '/Admin/LockUser',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            id: (document.getElementById('userIdInput') as HTMLInputElement).value,
        }),
        success: function (response) {
            getUserList_click();
        },
    });
}
function unlockUser_click() {
    $.ajax({
        url: urlPathBase + '/Admin/UnlockUser',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            id: (document.getElementById('userIdInput') as HTMLInputElement).value,
        }),
        success: function (response) {
            getUserList_click();
        },
    });
}
function deleteUser_click() {
    $.ajax({
        url: urlPathBase + '/Admin/DeleteUser',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            id: (document.getElementById('userIdInput') as HTMLInputElement).value,
        }),
        success: function (response) {
            getUserList_click();
        },
    });
}