<template>
  <div class="customer_list">
    <ul class="list">
      <li v-for="(item,index) in datalist" :key="index">
        <div class="company flex flex_a_c flex_s_b" @click="toggleMessage(item.uba_id)">
            <h2 class="name">{{ item.uba_personName }}</h2>
            <span>{{ item.uba_money }}</span>
        </div>
        <div class="message flex flex_a_c flex_s_b" v-show="item.isShow">
          <ul class="details_list">
            <li class="flex flex_a_c">
              <label class="title"><span>用途说明:</span></label>{{ item.uba_instruction }}
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>收款账户名称:</span></label>{{ item.uba_receiveBankName }}
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>预计借款日期:</span></label>{{ item.uba_foreDate | formatDate }}
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>实际借款日期:</span></label>{{ item.uba_date==null?"": (item.uba_date| formatDate) }}
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>出款银行:</span></label>{{ item.pm_name }}
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>审批状态:</span></label>
                <img class="icon" v-show="0 == item.uba_flag1" src="../../assets/img/audit.png" alt="">
                <img class="icon" v-show="1 == item.uba_flag1" src="../../assets/img/audit_no.png" alt="">
                <img class="icon" v-show="2 == item.uba_flag1" src="../../assets/img/audit_yes.png" alt="">
                <img class="icon" v-show="0 == item.uba_flag2" src="../../assets/img/audit.png" alt="">
                <img class="icon" v-show="1 == item.uba_flag2" src="../../assets/img/audit_no.png" alt="">
                <img class="icon" v-show="2 == item.uba_flag2" src="../../assets/img/audit_yes.png" alt="">
                <img class="icon" v-show="0 == item.uba_flag3" src="../../assets/img/audit.png" alt="">
                <img class="icon" v-show="1 == item.uba_flag3" src="../../assets/img/audit_no.png" alt="">
                <img class="icon" v-show="2 == item.uba_flag3" src="../../assets/img/audit_yes.png" alt="">
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>支付确认:</span></label>              
                <img class="icon" v-show="0 == item.uba_isConfirm" src="../../assets/img/audit.png" alt="">
                <img class="icon" v-show="2 == item.uba_isConfirm" src="../../assets/img/audit_yes.png" alt="">
            </li>
            <li class="flex flex_a_c">
              <label class="title"><span>图片:</span></label>{{ item.pm_name }}
              <ul>
                <li class="flex flex_a_c flex_s_b" v-for="(f,i) in item.piclist" :key="i">
                  <a :href="'/' + f.pp_filePath">{{f.pp_fileName}}</a>
                  <span style="margin-left:5px;">{{f.pp_size}}K</span>
                </li>
              </ul>
            </li>
          </ul>
        </div>
      </li>
    </ul>
    <top-nav title="执行备用金借款明细" ></top-nav>
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
      datalist:[]
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
		this.getunBusinessData()
  },
  methods: {
    ...mapActions([
      "getOrderunBusinessList",
      "getOrderunBusinessPic"
    ]),
    getunBusinessData() {
      this.getOrderunBusinessList({oid:this.orderID,managerid:this.userInfo.id}).then(res => {
        if (1 == res.data.status) {
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
    toggleMessage(ubaid){
      let newdatalist=[]
      this.datalist.some(item=>{
        if(item.uba_id ==ubaid){          
          if(true === item.isShow){
            item.isShow = false            
          }
          else{
            item.isShow = true
          }         
        }
        newdatalist.push(item)
      })
      this.datalist = newdatalist
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