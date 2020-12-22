<!-- 订单查询 -->
<template>
    <div>
        <tab-list :tabList="topTablist" @on-tab="changeTab"></tab-list>
        <label-search :list="labelData" :show="showLabel" @on-label="changeActive"></label-search>
		<div class="search_box flex flex_a_c flex_j_c" v-show="showSearchBox">
		   <input type="text" v-model="searchData.orderid" @blur="changeSearch" placeholder="搜索订单号、活动名称、活动地点">
		</div>
        <div class="menu_list flex flex_s_a">
            <div class="menu_top" @click="showSearchBoxChange">搜索</div>
            <div class="menu_top" @click="showLabel = true">筛选</div>
        </div>
        <div class="customer_list">
            <h2 class="amount">共{{recordTotal}}条</h2>
            <ul class="list">
				<!-- <router-link tag="li" :to="{ path: '/lookOrder',query: { id: item.o_id } }" v-for="(item,index) in showOrderList" :key="index"> -->
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
							<div v-show="0 == item.o_lockStatus" class="lock-status">未锁单</div>
							<div v-show="1 == item.o_lockStatus" class="lock-status green">已锁单</div>
				        </section>
				        <section class="operation_icon flex">
				            <span @click.prevent.stop="editOrder(item.o_id)"></span>
				            <span @click.prevent.stop="delListOrder(item.o_id)"></span>
				        </section>
				    </div>
				    <div class="message flex flex_a_c flex_s_b" style="display:block;height: 1.45rem">
				        <div class="message_list" style="display:block;float:left;">							
				            <span v-bind:class="item.o_status==0?'orderstatus_0':(item.o_status==1?'orderstatus_1':'orderstatus_2')">{{item.o_id}}</span>
				            <span>{{item.o_content}}</span>
				            <span>{{getListDate(item.o_sdate)}}/{{getListDate(item.o_edate)}}</span>
				            <span>{{item.o_address}}</span>
				        </div>
				        <div class="message_list1" style="display:block;float:left;">							
				            <span>应收:{{item.finMoney}}</span>
				            <span>未收:{{item.unMoney}}</span>
				            <span>业绩:{{item.profit}}</span>
				            <span>{{item.op_name}}{{item.op_number}}</span>
				        </div>
				    </div>
				</li>
				<!-- </router-link> -->
            </ul>
			<top-nav title="订单查询"></top-nav>
			<div class="loadmore" @click="loadNextPage" v-show="pageTotal > searchData.pageIndex">
				点击加载更多
			</div>
        </div>
    </div>
</template>

<script>
import tabList from '../../components/tab.vue'
import labelSearch from '../../components/labelSearch.vue'
import {
	mapActions,
	mapState
} from 'vuex'

export default {
    name:"",
    data() {
       return {
           topTablist:['我的订单','我的报账订单','我的执行订单','全公司的'],
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
		   pageTotal:9,
		   recordTotal:9,
		   searchData:{
			   pageIndex:0,
			   pageSize:10,
			   flag:1,
			   type:'',
			   orderid:'',
			   o_contractprice:'',
			   o_status:'',
			   o_dstatus:'',
			   o_ispush:'',
			   o_flag:'',
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
		...mapState(            
            {
            selectClientArray:state => state.addOrders.selectClientArray,
            userInfo: state => state.user.userInfo
        })},
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
		    'getAllcustomer',
		    'getContractprices',
		    'getContactsbycid',
		    'getPushstatus',
		    'getFstatus',
		    'getDstatus',
			'getLockStatus',
			'getOrderList',
			'delOrder'
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
			var tmpFlag = 1
			if(0 == index){
				tmpFlag = 1
			}
			if(1 == index){
				tmpFlag = 2
			}
			if(2 == index){
				tmpFlag = 4
			}
			if(3 == index){
				tmpFlag = 0
			}
			if(tmpFlag != this.searchData.flag){
				this.searchData.flag = tmpFlag
				this.newOrderList()
			}
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
			_this.searchData.managerid=14//_this.userInfo.id
            _this.ddSet.showLoad()
			this.getOrderList(this.searchData).then(function(res){                         
                _this.ddSet.hideLoad()
				if(res.data.msg){
					_this.recordTotal=0
					_this.ddSet.setToast({text:res.data.msg})
					return
				}
				if(_this.searchData.pageIndex < 2){
					_this.showOrderList = res.data.list;
					_this.recordTotal = res.data.pageTotal
					_this.pageTotal = Math.ceil(_this.recordTotal / _this.searchData.pageSize)
					console.log(_this.pageTotal)
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
				console.log(res)
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
        editOrder(_id){
            this.$router.push({path:'/modifyOrder', query: { id: _id }})
        },
		delListOrder(_id){
			let _this = this;
			_this.ddSet.setConfirm('确定要删除《'+_id+'》订单吗？').then(res=>{
				if(0 == res.buttonIndex){
					_this.ddSet.showLoad()
					_this.delOrder({
						orderID:_id,
						managerid:_this.searchData.managerid
					}).then((res) => {
						_this.ddSet.hideLoad()
						if (res.data.status) {
							_this.ddSet.setToast({text:'成功删除订单'})
							_this.showOrderList = _this.showOrderList.filter(function(item){
								return item.o_id != _id
							})
						}
						else{
							_this.ddSet.setToast({text:res.data.msg})
						}
					})
				}
			})
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
    
		.list{
			.message{
				.message_list{
					span:nth-child(1){
						//background-color: $blue_1;
						color: #ffffff;
					}
					span:nth-child(2){
						background-color: #fff4e9;
						color: #000000;
					}
					span:nth-child(3){
						background-color: #e3f6f0;
						color: #72a795;
					}
					span:nth-child(4){
						background-color: #ffffff;
    					color: #585809;
					}
				}
				.message_list1{
					display: flex;
					span{
						height: .36rem;
						font-size: $size_20;
						min-width: 1.22rem;
						border-radius: 2px;
						text-align: center;
						line-height: .36rem;
						margin-right: .1rem;
						// padding: 0 .1rem;
					}
					span:last-child{
						margin-right: 0;
					}
					span:nth-child(1){
						background-color: $blue_1;
						color: #FFF;
					}
					span:nth-child(2){
						background-color: #fff4e9;
						color: #dc143c;
					}
					span:nth-child(3){
						background-color: #e3f6f0;
						color: #72a795;
					}
					span:nth-child(4){
						background-color: #312d71;
    					color: #ffffcc;
					}
				}
			}
		}
	}
</style>

