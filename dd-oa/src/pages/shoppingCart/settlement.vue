<template>
  <div class="customer_list">
    <div class="list1">
      <span>应收总额：{{ this.resData.fin1 }}</span>
      <span>应付总额：{{ this.resData.fin0 }}</span>
      <span>利润：{{ this.resData.finProfit }}</span>
    </div>
    <div class="list1">
      <span>税费成本：{{ this.resData.finCust }}</span>
      <span>业绩利润：{{ this.resData.Profit }}</span>
    </div>
    <hr>
    <ul class="list">
      <li v-for="(item,index) in resData.list" :key="index">
        <div class="company flex flex_a_c flex_s_b">
            <h2 class="name">{{ item.c_name }}</h2>
            <span v-if="item.fin_type" style="color:blue">收</span>
            <span v-if="!item.fin_type" style="color:red">付</span>
        </div>
        <div class="message flex flex_a_c flex_s_b" >
          <div class="message_list1">
            <span>{{item.fin_type?"应收":"应付"}}:{{ item.finMoney }}</span>
            <span>{{item.fin_type?"已收":"已付"}}:{{ item.rpdMoney }}</span>
            <span>{{item.fin_type?"未收":"未付"}}:{{ item.unReceiptPay }}</span>
          </div>
        </div>
      </li>
    </ul>
    <top-nav title="结算汇总" ></top-nav>
  </div>
</template>

<script>
import { mapActions, mapState } from "vuex";
export default {
  data() {
    return {
      orderID: "",
      resData: {}
    };
  },
  created() {
    let { oID } = this.$route.query;
    this.orderID = oID;
  },
  computed: {
    ...mapState({
            userInfo: state => state.user.userInfo
        })
  },
  mounted() {
		this.getSettleMentList()
  },
  methods: {
    ...mapActions([
      "getOrderSettleMentList"
    ]),
    getSettleMentList() {
      this.getOrderSettleMentList({oid:this.orderID,managerid:this.userInfo.id}).then(res => {
        if (1 == res.data.status) {
          console.log(res.data.list)
          this.resData = res.data
        } else {
          this.ddSet.setToast({ text: res.data.msg });
        }
      });
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
      .message_list1{
        font-size: 14px;
        span{
          margin-right: 10px;
        }
      }
    }
  }
</style>