<!-- 业绩统计 -->
<template>
    <div>
        <label-search :list="labelData" :show="showLabel" @on-label="changeActive"></label-search>
        <div class="search_box flex flex_s_b flex_a_c" v-show="showSearchBox" @click="changeTime">
		   活动开始日期<span>{{startTime}}</span>-
		   <span>{{endTime}}</span>
		</div>
		<div class="search_box flex flex_s_b flex_a_c" v-show="showSearchBox" @click="changeTime1">
		   活动结束日期<span>{{startTime1}}</span>-
		   <span>{{endTime1}}</span>
		</div>
        <div class="search_box flex flex_s_b flex_a_c" v-show="showSearchBox">
		   未收金额<span @click="changesign">{{sign}}</span>
		   <input type="text" v-model="money" @change="changemoney($event)" @ placeholder="请输入未收金额">
		</div>
        <div class="search_box flex flex_s_b flex_a_c" v-show="showSearchBox" @click="changeArea">
		   <span>区域</span>
		   <input type="text" v-model="areaText" readonly>
           <div class="icon_right arrows_right"></div>
		</div>
        <div class="menu_list flex flex_s_a">
            <div class="menu_top" @click="showSearchBoxChange">搜索</div>
            <div class="menu_top" @click="clearSearchBox">清空搜索</div>
            <div class="menu_top" @click="showLabel = true">筛选</div>
        </div>
        <div class="customer_list" v-if="list.length">
            <h2 class="amount">
                共{{list.length}}条
                <span>共计未收款：{{totalMoney}}</span>
            </h2>
            <ul class="list">
                <li v-for="(item,index) in list" :key="index">
                    <div class="top flex">
                        <span class="name">{{item.op_number+`(${item.op_name})`}}</span>
                        <span>未收款：{{item.orderUnMoney}}</span>
                    </div>
                    <div class="bottom flex flex_s_a">
                        <span>应收款：{{item.orderFinMoney}}</span>
                        <span>订单已收款：{{item.orderRpdMoney}}</span>
                    </div>
                </li>
            </ul>
        </div>        
        <top-nav title="员工未收款"></top-nav>
			<div class="loadmore" @click="loadNextPage" v-show="pageTotal > pageIndex">
				点击加载更多
			</div>
    </div>
</template>

<script>
import {mapActions,mapState} from 'vuex'
import labelSearch from '../../components/labelSearch.vue'
import dayjs from 'dayjs'
export default {
    name:"",
    data() {
        return {
            list:[],
            signlist:[
                {
                    key:'>',
                    value:'>'
                },
                {
                    key:'>=',
                    value:'>='
                },
                {
                    key:'<',
                    value:'<'
                },
                {
                    key:'<=',
                    value:'<='
                },
                {
                    key:'<>',
                    value:'<>'
                }],
            tabIndex:0,
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
                }
            ],
            showLabel:false,
            showSearchBox:false,
            pageTotal:9,
            recordTotal:9,
            pageIndex:0,
			pageSize:10,
            totalMoney:0,
            startTime:null,
            endTime:null,
            startTime1:null,
            endTime1:null,
            status:2,
            lockstatus:null,
            sign:'>',
            money:'0',
            area:null,
            areaText:null
        };
    },
    components: {
        labelSearch
    },
    computed: {
        ...mapState({
            userInfo: state => state.user.userInfo
        })
    },
    mounted() {
        this.getLabelBaseData()
        this.getAreaSource()
    },
    methods: {
        ...mapActions([
            'getUnReceiveStatistic',
            'getLockStatus',
            'getFstatus',            
            'getArea',
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
                        if(index == 0){
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
                        else if(index == 1){
                            source = [{
                                isChecked:false,
                                text:'不限',
                                value:''
                            }]
                            item.data.map(function(lll){
                                if(lll.key == 2){
                                    source.push({
                                        isChecked:true,
                                        text:lll.value,
                                        value:lll.key
                                    })
                                }
                                else{
                                    source.push({
                                        isChecked:false,
                                        text:lll.value,
                                        value:lll.key
                                    })
                                }
                            })
                            _this.labelData[index]['list'] = source.concat()
                        }
					}
				})
			})
		},
        clearSearchBox(){
            let _this = this
            _this.startTime=null
            _this.endTime=null
            _this.startTime1=null
            _this.endTime1=null     
            _this.sign='>'
            _this.money='0'
            _this.area=null
            _this.areaText=null
            _this.newList()
        },
        changeArea(){    //区域
            let _this = this
            let source = []
            _this.getArea().then(res => {                
                if(_this.userInfo.area == 'HQ'){
                    source.push({
						key:'不限',
						value:''
                    })
                    res.data.map((item,index) => {
                        source.push({
                            key:item.value,
                            value:item.key
                        })
                    })
                }
                else{
                    res.data.map((item,index) => {
                        if(_this.userInfo.area == item.key ){
                            source.push({
                                key:item.value,
                                value:item.key
                            })
                        }
                    })
                }           
				let selectedKey = _this.areaText
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.area=res.value
                    _this.areaText = res.key
                    _this.newList()
                })
            })
        },
        getAreaSource(){
            let _this = this
            if(_this.userInfo.area == 'HQ'){
                _this.area=''
                _this.areaText='不限' 
                _this.newList()               
            }
            else{
                _this.getArea().then(res => {      
                    res.data.map((item,index) => {
                        if(_this.userInfo.area == item.key ){
                            _this.area=item.key
                            _this.areaText=item.value                            
                            _this.newList()
                        }
                    })                
                })
            }
        },
        changesign(){
            let _this = this
            let source = _this.signlist,selectedKey = _this.sign
            _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this,'sign',res.key)
                    _this.newList()
            })
        },
        changemoney(e){
            this.money = e.target.value
            this.newList()
        },
        changeTime(item){
            this.ddSet.setChooseInterval().then(res => {
                this.endTime = this.formatDate(res.end)
                this.startTime =this.formatDate(res.start)
                this.newList()
            })
        },
        changeTime1(item){
            this.ddSet.setChooseInterval().then(res => {
                this.endTime1 = this.formatDate(res.end)
                this.startTime1 =this.formatDate(res.start)
                this.newList()
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
        changeActive(actives){
            this.showLabel = false
            actives.map((item,index) => {
                if(item.key == 'status'){
                    this.status = item.value
                }
                else if(item.key == 'o_lockstatus'){
                    this.lockstatus = item.value
                }
            })
            this.newList()
        },
        showSearchBoxChange(){
			this.showSearchBox = !this.showSearchBox;
        },
		loadNextPage(){
			this.unReceiveStatisticList()
        },
        newList(){
			let _this = this
			_this.pageIndex = 0;
			_this.unReceiveStatisticList()
		},
        unReceiveStatisticList(){
            let _this = this    
            _this.pageIndex++  
            let params = {                
                sMonth:_this.startTime,
                eMonth:_this.endTime,
                sMonth1:_this.startTime1,
                eMonth1:_this.endTime1,
                status:_this.status,
                lockStatus:_this.lockstatus,
                sign:_this.sign,
                money1:_this.money,
                area:_this.area,
                pageIndex:_this.pageIndex,
                pageSize:_this.pageSize, 
                managerid:_this.userInfo.id
            }
            _this.ddSet.showLoad()
            _this.getUnReceiveStatistic(params).then(res => {
                _this.ddSet.hideLoad()
                if(res.data.msg){
                    _this.ddSet.setToast({text:res.data.msg})
                }
                if(_this.pageIndex < 2){
					_this.list = res.data.list
					_this.recordTotal = res.data.totalCount
					_this.pageTotal = Math.ceil(_this.recordTotal / _this.pageSize)
                    _this.totalMoney = res.data.totalMoney
				}
				else{
                    _this.list = _this.list.concat(res.data.list)
                    _this.totalMoney = res.data.totalMoney
                }
            })
        }
    },
}
</script>

<style scoped lang='scss'>
@import '../../assets/css/var.scss';
    .loadmore{text-align:center;padding:25px;font-size:.3rem;color:#888;background-color: #ededed}
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
        .amount span{
            margin-left: .5rem;
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
