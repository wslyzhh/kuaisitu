<template>
    <div class="">huaigui.y</div>
</template>

<script>
import {mapActions} from 'vuex'
import axios from 'axios'
export default {
mounted: function () {
        // GET /someUrl
        let params ={
            // "co_name":"杨一由",
            // "co_number":"1236645441223",
            // "co_id":74,
            // "o_content":"1111",
            // "o_address":"2222",
            // "o_place":"GZ,YN",
            // "employee1":"谢尧威|GZ003|GZ",
            // "employee2":"谢尧威|GZ003|GZ|0,陈睿智|YN001|YN|0",
            // "employee3":"杨飞|GZ002|GZ,谢尧威|GZ003|GZ,陈睿智|YN001|YN",
            // "employee4":"陈登晓|GZ001|GZ|0,杨飞|GZ002|GZ|0",
            // "o_remarks":"adfasdf",
            // "c_id":77,
            "managerid":14,
            
            // "fstatus":"0",
            // "orderID":"",
            // //"o_sdate":"2019-07-03",
            // "o_edate":"2019-07-05",
            // "o_contractprice":"50万以下",
            // "o_contractcontent":"",
            // "o_isPush":"True"
            // //"rp_id":78
        }
        axios.post('http://oa.zcenet.com/tools/dingtalk_ajax.ashx?action=unbusinesspay_auditBind',params).then(response => {
             console.log(response.data);
            // get body data
            // this.someData = response.body;
 
        }, response => {
            console.log("error");
        });
    }
}
</script>