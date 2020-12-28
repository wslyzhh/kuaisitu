<!-- 发票通知管理首页 -->
<template>
    <div>
        <tab-list :tabList="topTabList" @on-tab="changeTab"></tab-list>
        <div class="search_box flex flex_a_c flex_j_c">
           <input type="text" v-model="searchText" @input="changeSearch" placeholder="搜索凭证号或收款对象">
        </div>
        <div class="customer_list">
            <h2 class="amount">共{{list.length}}条</h2>
            <ul class="list">
                <li v-for="(item,index) in list" :key="index">                    
                    <div class="company flex flex_a_c flex_s_b">
                        <section class="flex flex_a_c">
                            <!-- <img class="icon" :src="isIocn[item.c_flag]" alt=""> -->
                            <router-link tag="span" :to="{path:'/billDetails',query:{id:item.inv_id}}"><h2 class="name">{{item.c_name}}</h2></router-link>
                            <input type="button" :class="{blue:item.inv_type}" :value="item.inv_type">
                            <input type="button" :class="{blue:item.inv_isConfirm}" :value="item.inv_isConfirm==false ? '未开票' : '已开票'">
                        </section>
                    </div>
                    <div class="message flex flex_a_c flex_s_b">
                        <div class="message_list flex">
                            <span v-bind:class="item.o_status==0?'orderstatus_0':(item.o_status==1?'orderstatus_1':'orderstatus_2')" v-show="item.inv_oid">{{item.inv_oid}}</span>
                            <span>开票金额：{{item.inv_money}}</span>
                        </div>
                    </div>
                </li>
            </ul>
        </div>
        <top-nav title="发票通知"></top-nav>
    </div>
</template>

<script>
import tabList from '../../components/tab.vue'
import {mapActions,mapState} from 'vuex'

export default {
    name:"",
    data() {
       return {
           topTabList:['未开票','已开票'],
           tabIndex:0,
           list:[],
           searchText:''
       };
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
        this.BillList()
    },
    methods: {
        ...mapActions([
            'getBillList'
        ]),
        BillList({inv_isConfirm = 0} = {}){
            let params = {
                pageIndex:1,
                pageSize:999,
                keywords:this.searchText,
                inv_isConfirm,
                managerid:this.userInfo.id
            }
            this.getBillList(params).then(res => {
                this.list = res.data.list
            })
        },
        changeSearch(){
            this.BillList({inv_isConfirm:this.tabIndex})
        },
        changeTab(index){
            this.tabIndex = index
            this.BillList({inv_isConfirm:index})
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
                        background-image: url('../../assets/img/redact_1.png');
                    }
                    span:nth-child(2){
                        background-image: url('../../assets/img/delete.png');
                        background-size: .25rem .3rem;
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


