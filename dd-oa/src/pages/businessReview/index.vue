<!-- 业务审核 -->
<template>
    <div>
		<!--  -->
        <tab-list :tabList="topTablist" @on-tab="changeTab"></tab-list>
        <label-search :list="labelData" :show="showLabel" @on-label="changeActive"></label-search>
		<div class="search_box flex flex_a_c flex_j_c" v-show="showSearchBox">
		   <input type="text" v-model="searchData.orderid" @input="changeSearch" placeholder="搜索订单号">
		</div>
        <div class="menu_list flex flex_s_a">
            <div class="menu_top" @click="showSearchBoxChange">搜索</div>
            <div class="menu_top" @click="showLabel = true">筛选</div>
        </div>
        <div class="customer_list">
            <h2 class="amount">共{{recordTotal}}条</h2>
            <ul class="list">
				<li v-for="(item,index) in showOrderList" :key="index">
				    <div class="company flex flex_a_c flex_s_b">
				        <section class="flex flex_a_c">
				            <img class="icon" v-show="0 == item.o_status" src="../../assets/img/audit.png" alt="">
							<img class="icon" v-show="1 == item.o_status" src="../../assets/img/audit_no.png" alt="">
							<img class="icon" v-show="2 == item.o_status" src="../../assets/img/audit_yes.png" alt="">
				            <h2 class="name">{{item.c_name}}</h2>
							<div v-show="0 == item.o_isPush" class="lock-status">未推送</div>
							<div v-show="1 == item.o_isPush" class="lock-status green">已推送</div>
							<div v-show="0 == item.o_flag" class="lock-status">待审</div>
							<div v-show="1 == item.o_flag" class="lock-status">未通过</div>
							<div v-show="2 == item.o_flag" class="lock-status green">通过</div>
							<div v-show="0 == item.o_lockStatus" class="lock-status ">未锁单</div>
							<div v-show="1 == item.o_lockStatus" class="lock-status green">已锁单</div>
				        </section>
				        <section class="operation_icon flex">
							<router-link tag="span" :to="{path:'/businessOrder',query:{id:item.o_id}}"></router-link>
				        </section>
				    </div>
				    <div class="message flex flex_a_c flex_s_b">
				        <div class="message_list flex">
				            <span>{{item.o_id}}</span>
				            <span>{{item.op_name}}{{item.op_number}}</span>
				            <span>{{item.finMoney}}</span>
				            <span>{{getListDate(item.o_sdate)}}/{{getListDate(item.o_edate)}}</span>
				        </div>
				    </div>
				</li>
            </ul>
			<div class="loadmore" @click="loadNextPage" v-show="pageTotal > searchData.pageIndex">
				点击加载更多
			</div>
        </div>
		<top-nav title="业务审批"></top-nav>
    </div>
</template>

<script>
import tabList from '../../components/tab.vue'
import labelSearch from '../../components/labelSearch.vue'
import {
	mapActions,
	mapState
} from 'vuex'
import * as dd from 'dingtalk-jsapi'

export default {
    name:"",
    data() {
       return {
           topTablist:['待审批','已审批'],
           showLabel:false,
           showSearchBox:false,
		   labelData:[
			   {
			       title:'合同造价',
			       class:'greenTitle',
				   s_key:'o_contractprice',
			       list:[]
			   },
			   {
			       title:'锁单状态',
					class:'blueTitle',
					s_key:'o_lockstatus',
			       list:[]
			   },
			   {
			       title:'订单状态',
					class:'',
					s_key:'o_status',
			       list:[]
			   },
		   ],
		   showOrderList:[],
		   pageTotal:0,
		   recordTotal:0,
		   searchData:{
			   pageIndex:0,
			   pageSize:10,
			   flag:0,
			   type:'check',
			   orderid:'',
			   o_contractprice:'',
			   o_status:'',
			   o_ispush:'True',
			   o_flag:'0',
			   o_lockstatus:'',
			   managerid:0     // TODO: 测试用，后面注意修改
		   }
       };
    },
    components: {
        tabList,
        labelSearch
    },
    computed: {	
        ...mapState({
            userInfo: state => state.user.userInfo
		})
	},
    created(){
        // this.ddSet.setTitleRight({title:'订单查询'}).then(res => {
        //     if(res){

        //     }
        // })
    },
    mounted() {
		this.newOrderList()
		this.getLabelBaseData()
    },
    methods: {
		...mapActions([
		    'getAllCustomer',
		    'getContractprices',
		    'getContactsbycid',
		    'getPushstatus',
		    'getFstatus',
		    'getDstatus',
			'getLockStatus',
			'getOrderList'
		]),
		showSearchBoxChange(){
			this.showSearchBox = !this.showSearchBox;
			// 不显示时，清空关键词
			if(!this.showSearchBox && this.searchData.orderid){
				this.searchData.orderid = '';
				this.newOrderList()
			}
		},
        changeTab(index){
			if(0 == index){
				this.searchData.o_flag = 0
			}
			if(1 == index){
				this.searchData.o_flag = 3
			}
			this.newOrderList()
        },
        changeActive(actives){
			let _this = this
            this.showLabel = false
			actives.map((item) => {
				_this.searchData[item.key] = item.value;
			})
			this.newOrderList()
        },
		loadNextPage(){
			this.orderList()
		},
		changeSearch(){
		    this.newOrderList()
		},
		orderList(){
			let _this = this
			_this.searchData.pageIndex++
			_this.searchData.managerid = _this.userInfo.id
			this.getOrderList(this.searchData).then(function(res){
				if(res.data.msg){
					_this.recordTotal = 0
					_this.ddSet.setToast({text:res.data.msg})
					return
				}
				if(_this.searchData.pageIndex < 2){
					_this.showOrderList = res.data.list
					_this.recordTotal = res.data.pageTotal
					_this.pageTotal = Math.ceil(_this.recordTotal / _this.searchData.pageSize)
				}
				else{
					_this.showOrderList = _this.showOrderList.concat(res.data.list);
				}
			})
		},
		newOrderList(){
			let _this = this
			_this.searchData.pageIndex = 0;
			_this.orderList()
		},
		getLabelBaseData(){
			let _this = this
			Promise.all([
				_this.getContractprices({ddkey:'dingzreafyvgzklylomj'}),// 合同造价
				_this.getLockStatus({ddkey:'dingzreafyvgzklylomj'}),// 锁单状态
				_this.getFstatus({ddkey:'dingzreafyvgzklylomj'})// 订单状态
			]).then(function(res){
				let source = []
				res.map(function(item,index){
					if(200 == item.status){
						source = [{
							isChecked:true,
							text:'不限',
							value:''
						}]
						item.data.map(function(lll){
							source.push({
								isChecked:false,
								text:lll.value,
								value:lll.key
							})
						})
						_this.labelData[index]['list'] = source.concat()
					}
				})
			})
		},
        edit(_id){
            this.$router.push({path:'/businessOrder',query: {id:_id}})
        },
		// 处理 时间
		getListDate(_date){
			if(!_date){
				return '';
			}
			let tmp = _date.split('T');
			return tmp[0]
		}
    },
}
</script>

<style scoped lang="scss">
@import '../../assets/css/var.scss';
    .search_box{
        height: .88rem;
        input{
            width: 7rem;
            height: .6rem;
            text-indent: .5rem;
            background-color: #ededed;
            border-radius: 4px;
            font-size: $size_28;
            background-image: url('../../assets/img/search.png');
            background-repeat: no-repeat;
            background-size: .3rem;
            background-position:.1rem;
        }
    }
    .menu_list{
        padding: .22rem 0;
        .menu_top{
            flex: 1;
            font-size: .28rem;
            text-align: center;
            line-height: .35rem;
            color: $fontSizeColor;
            &:first-child{
                border-right: 1px solid #efefef;
            }
        }
        
    }
	.loadmore{text-align:center;padding:25px;font-size:.3rem;color:#888;background-color: #ededed}
	.lock-status{
		width: .86rem;
		height: .36rem;
		line-height: .36rem;
		margin-left: .1rem;
		font-size: 0.2rem;
		color: #FFF;
		border-radius: .04rem;
		background-color: #a6a6a6;
		text-align: center;
	}
	.lock-status.green{background-color:#55be17;}
	
	.customer_list{
        .amount{
            height: .65rem;
            line-height: .65rem;
            padding: 0 .3rem;
            font-size: $size_28;
            color: $gery_1;
            font-weight: normal;
            background-color: $gery_3;
        }
        .list{
            .company{
                .operation_icon{
                    span{
                        width: $size_30;
                        height: $size_30;
                        margin-left: .3rem;
                        background-repeat: no-repeat;
                        background-position: center;
                        background-size: cover;
                    }
                    span:nth-child(1){
                        background-image: url('../../assets/img/auditting.png');
                    }                    
                }
            }
        }
    }
</style>
