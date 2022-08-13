<!-- 业绩统计 -->
<template>
    <div class="body_top">
        <tab-list tabClass="fixed" :tabList="topTablist" @on-tab="changeTab"></tab-list>
        <label-search :list="labelData" :show="showLabel" @on-label="changeActive"></label-search>
		<div class="search_box flex flex_s_b flex_a_c" v-show="showSearchBox">
		   订单结束日期<span @click="changeTime">{{startTime}}</span>-
		   <span @click="changeTime">{{endTime}}</span>
		</div>
        <div class="menu_list flex flex_s_a">
            <div class="menu_top" @click="showSearchBoxChange">搜索</div>
            <div class="menu_top" @click="showLabel = true">筛选</div>
        </div>
        <div class="customer_list" v-if="list.length">
            <h2 class="amount">共{{list.length}}条</h2>
            <ul class="list">
                <li v-for="(item,index) in list" :key="index">
                    <div class="top flex">
                        <span class="name">{{item.op_number+`(${item.op_name})`}}</span>
                        <span class="address">{{item.op_area}}</span>
                        <span>数量：{{item.oCount}}</span>
                    </div>
                    <div class="bottom flex flex_s_a">
                        <span>提成前业绩：{{item.profit1}}</span>
                        <span>提成前业绩率：{{item.profitRatio1}}%</span>
                    </div>
                    <div class="bottom flex flex_s_a">
                        <span>提成后业绩：{{item.profit2}}</span>
                        <span>提成后业绩率：{{item.profitRatio2}}%</span>
                    </div>
                </li>
            </ul>
        </div>        
        <top-nav title="员工业绩统计"></top-nav>
    </div>
</template>

<script>
import {mapActions,mapState} from 'vuex'
import tabList from '../../components/tab.vue'
import labelSearch from '../../components/labelSearch.vue'
import dayjs from 'dayjs'
export default {
    name:"",
    data() {
        return {
            list:[],
            tabIndex:0,
            topTablist:['下单','策划接单','设计接单'],
            labelData:[
                {
                    title:'锁单状态',
                    class:'blueTitle',
                    s_key:'o_lockstatus',
                    list:[]
                },
                {
                    title:'订单状态',
                    class:'greenTitle',
                    s_key:'status',
                    list:[]
                },
                {
                    title:'是否包含税费成本',
                    class:'',
                    s_key:'isCust',
                    list:[
                        {
                            isChecked:true,
                            text:'是',
                            value:true
                        },
                        {
                            isChecked:false,
                            text:'否',
                            value:false
                        },
                    ]
                },
            ],
            showLabel:false,
            showSearchBox:false,
            startTime:null,
            endTime:null,
            isCust:true,
            status:null,
            lockStatus:null
        };
    },
    components: {
        tabList,
        labelSearch
    },
    computed: {
        ...mapState({
            userId:state => state.user.userInfo.id
        })
    },
    mounted() {
        this.achievementStatisticList()
        this.getLabelBaseData()
    },
    methods: {
        ...mapActions([
            'getAchievementStatistic',
            'getLockStatus',
            'getFstatus',
        ]),
        getLabelBaseData(){
			let _this = this
			Promise.all([
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
        getNowFormatDate(num = 1) {
            var date = new Date();
            var seperator1 = "-";
            var year = date.getFullYear();
            var month = date.getMonth() + num;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = year + seperator1 + month + seperator1 + strDate;
            return currentdate;
        },
        changeTime(item){
            this.ddSet.setChooseInterval().then(res => {
                this.endTime = this.formatDate(res.end)
                this.startTime =this.formatDate(res.start)
                this.achievementStatisticList()
            })
        },
        formatDate(data) {
            data = new Date(data)
            let year = data.getFullYear();
            let month = data.getMonth()+1;
            let day = data.getDate();
            if(month<10){
                month='0'+month;
            }
            if(day<10){
                day='0'+day;
            }
            return `${year}-${month}-${day}`
        },
        changeTab(index){
            this.tabIndex = index
            this.achievementStatisticList()
        },
        changeActive(actives){
            this.showLabel = false
            actives.map((item,index) => {
                if(item.key == 'isCust'){
                    this.isCust = item.value
                }else if(item.key == 'status'){
                    this.status = item.value
                }
                else if(item.key == 'o_lockstatus') 
                {
                    this.lockStatus = item.value
                }
            })
            this.achievementStatisticList()
        },
        showSearchBoxChange(){
			this.showSearchBox = !this.showSearchBox;
        },
        achievementStatisticList(){
            if(!this.endTime && !this.startTime){
                this.endTime = this.getNowFormatDate()
                this.startTime = this.getNowFormatDate(0)
            }
            let managerid =this.userId
            let params = {
                type:this.tabIndex,
                sMonth:this.startTime,
                eMonth:this.endTime,
                status:this.status,
                isCust:this.isCust,
                lockStatus:this.lockStatus,
                pageIndex:1,
                pageSize:10, 
                managerid
            }
            this.ddSet.showLoad()
            this.getAchievementStatistic(params).then(res => {
                this.ddSet.hideLoad()
                if(res.data.msg){
                    this.ddSet.setToast({text:res.data.msg})
                }else{
                    this.list = res.data.list
                }
            })
        }
    },
}
</script>

<style scoped lang='scss'>
@import '../../assets/css/var.scss';
    .customer_list{
        .list{
            li{
                padding: $size_28;
                border-bottom: 1px solid $border_bottom_color;
            }
            span{
                font-size: .28rem;
                color: $fontSizeColor;
            }
            .top{
                span{
                    margin-right: .3rem;
                }
            }
            .bottom{
                margin-top: .3rem;
                span{
                    flex: 1;
                    color: $gery_1;
                }
            }
        }
    }
    .search_box{
        font-size: .28rem;
        color: #333;
        padding:0 .3rem;
        border-bottom: 1px solid #f1f1f1;
        background-color: #FFF;
        span{
            color: #666;
            height: .8rem;
            line-height: .8rem;
        }
    }
    .menu_list{
        padding: .22rem 0;
        background-color:#FFF;
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
</style>
