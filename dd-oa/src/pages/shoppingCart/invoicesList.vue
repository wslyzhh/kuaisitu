<template>
  <div class="customer_list">
    <div class="list1">
      <span>已申请：{{ this.resData.requestMoney }}</span>
      <span>已开具：{{ this.resData.confirmMoney }}</span>
      <span>剩余可申请：{{ this.resData.leftInvMoney }}</span>
    </div>
    <hr>
    <ul class="list">
      <li v-for="(item,index) in datalist" :key="index">
        <div class="company flex flex_a_c flex_s_b" @click="toggleMessage(item.inv_id)">
            <h2 class="name">{{ item.c_name }}</h2>
            <span>{{ item.inv_money }}</span>
        </div>
        <div class="message flex flex_a_c flex_s_b" v-show="item.isShow">
          <ul class="details_list">
            <li class="flex flex_a_c">
              <label class="title"><span>开票项目:</span></label>{{ item.inv_serviceType }}/{{ item.inv_serviceName }}
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>专普票:</span></label>{{ item.inv_type?"专票":"普票" }}
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>申请时超开:</span></label>{{ item.inv_overMoney }}
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>送票方式:</span></label>{{ item.inv_sentWay }}
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>开票区域:</span></label>{{ item.inv_darea }}
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>申请人:</span></label>{{ item.inv_personName }}/{{ item.inv_personNum }}
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>审批状态:</span></label>
              <img class="icon" v-show="0 == item.inv_flag1" src="../../assets/img/audit.png" alt="">
							<img class="icon" v-show="1 == item.inv_flag1" src="../../assets/img/audit_no.png" alt="">
							<img class="icon" v-show="2 == item.inv_flag1" src="../../assets/img/audit_yes.png" alt="">
              <img class="icon" v-show="0 == item.inv_flag2" src="../../assets/img/audit.png" alt="">
							<img class="icon" v-show="1 == item.inv_flag2" src="../../assets/img/audit_no.png" alt="">
							<img class="icon" v-show="2 == item.inv_flag2" src="../../assets/img/audit_yes.png" alt="">
              <img class="icon" v-show="0 == item.inv_flag3" src="../../assets/img/audit.png" alt="">
							<img class="icon" v-show="1 == item.inv_flag3" src="../../assets/img/audit_no.png" alt="">
							<img class="icon" v-show="2 == item.inv_flag3" src="../../assets/img/audit_yes.png" alt="">
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>开票状态:</span></label>              
              <img class="icon" v-show="false == item.inv_isConfirm" src="../../assets/img/audit.png" alt="">
							<img class="icon" v-show="true == item.inv_isConfirm" src="../../assets/img/audit_yes.png" alt="">
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>开票日期:</span></label>{{ item.inv_date | formatDate}}
            </li>
          </ul>
        </div>
      </li>
    </ul>
    <top-nav title="发票申请汇总" ></top-nav>
  </div>
</template>

<script>
import { mapActions, mapState } from "vuex";
import {formatDate} from '../../assets/js/date.js'
export default {
  data() {
    return {
      orderID: "",
      resData: {},
      datalist:[],
      newdatalist:[]
    };
  },
  created() {
    let { oID } = this.$route.query;
    this.orderID = oID;
  },
  filters:{
        formatDate(time){
            let date = new Date(time)
            return formatDate(date,'yyyy-MM-dd')
        }
  },
  computed: {
    ...mapState({
            userInfo: state => state.user.userInfo
        })
  },
  mounted() {
		this.getInvoiceList()
  },
  methods: {
    ...mapActions([
      "getOrderInvoiceList"
    ]),
    getInvoiceList() {
      this.getOrderInvoiceList({oid:this.orderID,managerid:this.userInfo.id}).then(res => {
        if (1 == res.data.status) {
          console.log(res.data.list)
          this.resData = res.data
          this.resData.list.forEach(element => {
              element.isShow = false
              this.datalist.push(element)
          })
        } else {
          this.ddSet.setToast({ text: res.data.msg });
        }
      });
    },
    toggleMessage(invid){
      this.newdatalist=[]
      this.datalist.some(item=>{
        if(item.inv_id ==invid){
          if(true === item.isShow){
            item.isShow=false
          }
          else{
            item.isShow = true
          }          
        }
        this.newdatalist.push(item)
      })
      this.datalist = this.newdatalist
    }
  }
};
</script>

<style scoped lang="scss">
  .customer_list{
    .list1{
      font-size: 14px;
      padding: 10px;
      span{
        margin-right: 10px;
      }
    }
    hr{
      border:1px solid #ccc;
      height: 0px;
    }
    .list{
      .company{
        border-bottom: 1px solid gray;
      }
      .message{
        padding: 5px 10px;
        font-size: 12px;
        height: 3.5rem;
        .details_list{
          li{
            line-height: 20px;
          }
        }
        .icon{
          background: none;
          margin-right: 2px;
        }
      }
    }
  }
</style>