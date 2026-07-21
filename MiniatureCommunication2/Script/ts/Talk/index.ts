import '../../../Style/scss/Talk/index.scss';
import $ from 'jquery';

import ConversationDTO_CrMember from '../../types/MC2/Database/ConversationDTO_CrMember.type';
export function init() {
    (window as any).conversationList_getList = conversationList_getList;
}

function conversationList_getList() {
    $.ajax({
        url: urlPathBase + '/Talk/GetConversationList',
        type: "GET",
        contentType: "application/json",
        success: function (res:ConversationDTO_CrMember[]) {
            //console.log(res);
            const cl = document.getElementById('conversationList');
            cl.replaceChildren();//清除所有子元素
            res.forEach(c => {
                const elem = document.createElement('input');
                elem.type = 'button';
                elem.value = c.group_Title;
                elem.onclick = () => { console.log(c.id) };

                cl.appendChild(elem);
            })
        },
    });
}
