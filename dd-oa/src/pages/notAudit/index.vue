<!-- 付款明细列表首页 -->
<template>
    <div>
        <tab-list :tabList="topTabList" @on-tab="changeTab"></tab-list>
        <div class="search_box flex flex_a_c flex_j_c">
           <input type="text" v-model="searchData.keywords" @input="changeSearch" placeholder="搜索客户名称">
        </div>
        <div class="customer_list">
            <h2 class="amount">共{{recordTotal}}条</h2>
            <ul class="list">
                <router-link tag="li" :to="{path:'/notAuditDetails',query: {rpd_id:item.rpd_id,type:'Check'}}" v-for="(item,index) in showPayList" :key="index">
                    <div class="company flex flex_a_c flex_s_b">
                        <section class="flex flex_a_c">                            
                            <div v-if='checkType===1'>
                                <img class="icon" :src="isIocn[item.rpd_flag1]" alt="11">
                            </div>
                            <div v-else-if='checkType===2'>
                                <img class="icon" :src="isIocn[item.rpd_flag2]" alt="22">
                            </div>
                            <div v-else>
                                <img class="icon" :src="isIocn[item.rpd_flag3]" alt="33">
                            </div>
                            <h2 class="name">{{item.c_name}}</h2>
                            <!-- <input type="button" :class="{blue:item.rp_isConfirm}" :value="item.rp_isConfirm?'已收款':'未收款'" class="blueq">
                            <span class="isExpect">{{item.rp_isExpect?'[预]':''}}</span> -->
                        </section>
                        <section class="operation_icon flex">
                            <router-link tag="span" :to="{path:'/notAuditDetails',query:{rpd_id:item.rpd_id,type:'Check'}}"></router-link>
                        </section>
                    </div>
                    <div class="message flex flex_a_c flex_s_b">
                        <div class="message_list flex">
                            <span v-bind:class="item.o_status==0?'orderstatus_0':(item.o_status==1?'orderstatus_1':'orderstatus_2')">{{item.rpd_oid}}</span>
                            <span>{{item.rpd_money}}</span>
                            <span>{{item.rpd_foredate | formatDate}}</span>
                            <span>{{item.rpd_personNum}}({{item.rpd_personName}})</span>
                        </div>
                    </div>
                </router-link>
            </ul>
            <div class="loadmore" @click="loadNextPage" v-show="pageTotal > searchData.pageIndex">
				点击加载更多
			</div>
        </div>
        <top-nav title="付款通知"></top-nav>
    </div>
</template>

<script>
import tabList from '../../components/tab.vue'
import {mapActions,mapState} from 'vuex'
import {formatDate} from '../../assets/js/date.js'

import audit from '../../assets/img/audit.png'
import audit_no from '../../assets/img/audit_no.png'
import audit_yes from '../../assets/img/audit_yes.png'
export default {
    name:"",
    data() {
       return {
           topTabList:['待审批','已审批'],
           isIocn:[audit,audit_no,audit_yes],
           showPayList:[],
           checkType:0,
           pageTotal:9,
		   recordTotal:9,
		   searchData:{
			    pageIndex:1,
                pageSize:10,
                keywords:'',
                type:'check',
                flag:'0',
                managerid:0
		   }
       };
    },
    filters:{
        formatDate(time){
            let date = new Date(time)
            return formatDate(date,'yyyy-MM-dd')
        }
    },
    components: {
        tabList,
    },
    computed: {
        ...mapState({
            userInfo: state => state.user.userInfo
        })
    },
    created(){},
    mounted() {        
        this.newpayDetailList()
    },
    methods: {
        ...mapActions([
            'getPayDetailList'
        ]),
        loadNextPage(){
			this.payDetailList()
		},
        payDetailList(){            
            let _this = this
            _this.searchData.pageIndex++
            _this.searchData.managerid=_this.userInfo.id
            this.getPayDetailList(_this.searchData).then(res => {
                if(res.data.msg){
                    _this.recordTotal=0
					_this.ddSet.setToast({text:res.data.msg})
					return
				}
				if(_this.searchData.pageIndex < 2){
					_this.showPayList = res.data.list;
					_this.recordTotal = res.data.pageTotal
                    _this.pageTotal = Math.ceil(_this.recordTotal / _this.searchData.pageSize)
                    _this.checkType = res.data.checkType
				}
				else{
					_this.showPayList = _this.showPayList.concat(res.data.list);
				}
            })
        },
        newpayDetailList(){
			let _this = this
			_this.searchData.pageIndex = 0;
			_this.payDetailList()
		},
        changeSearch(){
            this.newpayDetailList()
        },
        changeTab(index){
            this.searchData.flag = index;
			this.newpayDetailList()
        }
    },
}
</script>

<style scoped lang="scss">
@import '../../assets/css/var.scss';
    .tab_list{
        border-bottom: 1px solid $border_bottom_color;
        li{
            height: .75rem;
            line-height: .75rem;
            color:$fontSizeColor;
            font-size: $size_30;
        }
        .active{
            color:$blue_1;
            font-weight: bold;
            border-bottom: 2px solid $blue_1;
        }
    }
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
                        //background-color: $blue_1;
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


