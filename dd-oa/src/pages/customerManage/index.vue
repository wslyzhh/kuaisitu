<!-- 客户管理首页 -->
<template>
    <div>
        <tab-list :tabList="topTabList" @on-tab="changeTab"></tab-list>
        <div class="search_box flex flex_a_c flex_j_c">
           <input type="text" v-model="searchData.keywords" @input="changeSearch" placeholder="搜索联系人姓名或公司名称">
        </div>
        <div class="customer_list">
            <h2 class="amount">共{{recordTotal}}人</h2>
            <ul class="list">
                <li v-for="(item,index) in showCustomerList" :key="index" @click="goPage('clientDetails',item.c_id)">
                    <div class="company flex flex_a_c flex_s_b">
                        <section class="flex flex_a_c">
                            <img class="icon" :src="isIocn[item.c_flag]" alt="">
                            <h2 class="name">{{item.c_name}}</h2>
                            <input type="button" :class="{blue:item.c_isUse}" :value="item.c_isUse?'启用': '禁用'">
                        </section>
                        <section class="operation_icon flex" v-if="item.c_flag !== 2">
                            <span @click.stop="goPage('addClient',item.c_id)"></span>
                            <span @click.prevent.stop="delClient(item.c_id)"></span>
                        </section>
                    </div>
                    <div class="message flex flex_a_c flex_s_b">
                        <div class="message_list flex">
                            <span v-show="item.c_num">{{item.c_num}}</span>
                            <span>{{item.co_name}}</span>
                            <span>{{item.co_number}}</span>
                        </div>
                    </div>
                </li>
            </ul>
			<div class="loadmore" @click="loadNextPage" v-show="pageTotal > searchData.pageIndex">
				点击加载更多
			</div>
        </div>
        <top-nav title="客户管理" text='添加客户' @rightClick="addClient"></top-nav>
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
           showCustomerList:[],
		   pageTotal:9,
		   recordTotal:9,
           topTabList:['普通客户','管理用客户','内部客户'],
           tabIndex:0,
           list:[],
           isIocn:[audit,audit_no,audit_yes],
           searchText:'',
           searchData:{
			   pageIndex:0,
			   pageSize:10,
			   keywords:'',
			   managerid:0     // TODO: 测试用，后面注意修改
		   }
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
    created(){
        
    },
    mounted() {
        this.newCustomerList()
    },
    methods: {
        ...mapActions([
            'getCustomerList',
            'getCustomerDel'
        ]),
        addClient(){
            this.$router.push({path:'/addClient',query:{type:'add'}})
        },
        goPage(item,params){
            if(item == 'clientDetails'){
                this.$router.push({path:`/${item}`,query:{id:params}})
            }
            if(item == 'addClient'){
                this.$router.push({path:`/${item}`,query:{c_id:params,type:'EDIT'}})
            }
        },
        delClient(_id){
            let _this = this;
			_this.ddSet.setConfirm('确定要删除吗？').then(res=>{
				if(0 == res.buttonIndex){
					_this.ddSet.showLoad()
					_this.getCustomerDel({
						c_id:_id,
						managerid:_this.userInfo.id
					}).then((res) => {
						_this.ddSet.hideLoad()
						if (res.data.status) {
							_this.ddSet.setToast({text:'删除成功'})
							_this.showCustomerList = _this.showCustomerList.filter(function(item){
								return item.c_id != _id
							})
						}
						else{
							_this.ddSet.setToast({text:res.data.msg})
						}
					})
				}
			})
        },
        loadNextPage(){
			this.customerList({type:this.tabIndex+1})
		},
        customerList({type = 1} = {}){
            let _this = this
			_this.searchData.pageIndex++
            _this.searchData.managerid = 14//_this.userInfo.id
            _this.searchData.type = type
			_this.ddSet.showLoad()
			_this.getCustomerList(_this.searchData).then(function(res){
				_this.ddSet.hideLoad()
				console.log(res.data)
				if(res.data.msg){
					_this.ddSet.setToast({text:res.data.msg})
					return
				}
				if(_this.searchData.pageIndex < 2){
					_this.showCustomerList = res.data.list;
					_this.recordTotal = res.data.pageTotal
					_this.pageTotal = Math.ceil(_this.recordTotal / _this.searchData.pageSize)
				}
				else{
					_this.showCustomerList = _this.showCustomerList.concat(res.data.list);
				}
			})
        },
        newCustomerList(){
			let _this = this
			_this.searchData.pageIndex = 0;
			_this.customerList({type:this.tabIndex+1})
		},	
        changeSearch(){
            this.newCustomerList()
        },
        changeTab(index){
            this.tabIndex = index
            this.newCustomerList()
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
</style>


