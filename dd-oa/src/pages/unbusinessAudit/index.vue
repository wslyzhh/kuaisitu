<!-- 非业务支付审批 -->
<template>
    <div>
		<tab-list :tabList="topTabList" @on-tab="changeTab"></tab-list>
		<div class="search_box flex flex_a_c flex_j_c">
		   <input type="text" v-model="searchData.keywords" @input="changeSearch" placeholder="模糊搜索订单号或支付用途">
		</div>
        <div class="customer_list">
            <h2 class="amount">共{{recordTotal}}条</h2>
            <ul class="list">
                <router-link tag="li" :to="{path:'/unbusinessDetails',query: {id:item.uba_id,type:'check'}}" v-for="(item,index) in showUnBusinessPayList" :key="index">
				    <div class="company flex flex_a_c flex_s_b">
                        <section class="flex flex_a_c">
							<div v-if='checkType===1'>
                                <img class="icon" :src="isIocn[item.uba_flag1]" alt="11">
                            </div>
                            <div v-else-if='checkType===2'>
                                <img class="icon" :src="isIocn[item.uba_flag2]" alt="22">
                            </div>
                            <div v-else>
                                <img class="icon" :src="isIocn[item.uba_flag3]" alt="33">
                            </div>
                            <h2 class="name">{{item.uba_function}}</h2>
							<input type="button" :class="{blue:item.uba_isConfirm}" :value="item.uba_isConfirm?'已支付': '未支付'">
                        </section>
						<section class="operation_icon flex">
                            <router-link tag="span" :to="{path:'/unbusinessDetails',query:{id:item.uba_id,type:'check'}}"></router-link>
                        </section>
				    </div>
				    <div class="message flex flex_a_c flex_s_b">
				        <div class="message_list flex">
				            <span v-bind:class="item.o_status==0?'orderstatus_0':(item.o_status==1?'orderstatus_1':'orderstatus_2')" v-show="item.uba_oid">{{item.uba_oid}}</span>
				            <span>{{item.uba_money}}</span>
				            <span>{{item.uba_PersonNum}}({{item.uba_personName}})</span>
				        </div>
				    </div>
				</router-link>
            </ul>
			<div class="loadmore" @click="loadNextPage" v-show="pageTotal > searchData.pageIndex">
				点击加载更多
			</div>
        </div>
        <top-nav title="非业务支付审批"></top-nav>
    </div>
</template>

<script>
import tabList from '../../components/tab.vue'
import {mapActions,mapState} from 'vuex'
import audit from '../../assets/img/audit.png'
import audit_no from '../../assets/img/audit_no.png'
import audit_yes from '../../assets/img/audit_yes.png'

export default {
    name:"",
    data() {
       return {
           topTabList:['待审核','已审核'],  
           isIocn:[audit,audit_no,audit_yes],
		   showUnBusinessPayList:[],
           checkType:0,
		   pageTotal:9,
		   recordTotal:9,
		   searchData:{
			   	pageIndex:0,
			   	pageSize:10,
			   	keywords:'',
				type:'check',
				flag:0,
			   	managerid:0     // TODO: 测试用，后面注意修改
		   	}
       };
    },
    components: {
        tabList
    },
    computed: {		
        ...mapState({
            userInfo: state => state.user.userInfo
        })
	},
    created(){
    },
    mounted() {
		this.newUnBusinessPayList()
    },
    methods: {
		...mapActions([
			'getUnBusinessPayList'
		]),	
        changeTab(index){
			this.searchData.flag = index
			this.newUnBusinessPayList()
        },
		loadNextPage(){
			this.UnBusinessPayList()
		},
		changeSearch(){
		    this.newUnBusinessPayList()
		},
		UnBusinessPayList(){
			let _this = this
			_this.searchData.pageIndex++
			_this.searchData.managerid = 17//_this.userInfo.id
			this.getUnBusinessPayList(this.searchData).then(function(res){
				console.log(res.data)
				if(res.data.msg){
					_this.ddSet.setToast({text:res.data.msg})
					return
				}
				if(_this.searchData.pageIndex < 2){
					_this.showUnBusinessPayList = res.data.list;
					_this.recordTotal = res.data.pageTotal
					_this.pageTotal = Math.ceil(_this.recordTotal / _this.searchData.pageSize)
                    _this.checkType = res.data.checkType
				}
				else{
					_this.showUnBusinessPayList = _this.showUnBusinessPayList.concat(res.data.list);
				}
			})
		},
		newUnBusinessPayList(){
			let _this = this
			_this.searchData.pageIndex = 0;
			_this.UnBusinessPayList()
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
                padding: 0 .3rem;
                height: .65rem;
                .icon{
                    width:$size_30;
                    height: $size_30;
                    background-color: $gery_3;
                    margin-right: $size_20;
                }
                .name{
                    color: $blue_1;
                    font-size: $size_24;
                }
                .isExpect{
                    font-size: $size_20;
                    margin-left: .1rem;
                    color:green;
                }
                input{
                    width: .86rem;
                    height: .36rem;
                    line-height: .36rem;
                    margin-left: .1rem;
                    font-size: $size_20;
                    color: #FFF;
                    border-radius: .04rem;
                    background-color: $gery_2;
                }
                .blue{
                    background-color: $blue_1;
                }
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
            .message{
                padding: 0 .3rem;
                height: .65rem;
                background-color: $gery_3;
                .icon{
                    width: $size_30;
                    height: $size_30;
                    background-color: $blue_1;
                }
                .message_list{
                    display: flex;
                    span{
                        height: .36rem;
                        font-size: $size_20;
                        min-width: 1.22rem;
                        border-radius: 2px;
                        text-align: center;
                        line-height: .36rem;
                        margin-right: .2rem;
                    }
                    span:nth-child(1){
                        background-color: $blue_1;
                        color: #FFF;
                        padding: 0 .2rem;
                    }
                    span:nth-child(2){
                        background-color: #fff4e9;
                        color: #e1bc94;
                    }
                    span:nth-child(3){
                        background-color: #e3f6f0;
                        color: #72a795;
                        width: 1.8rem;
                    }
                }
            }
        }
    }
</style>

