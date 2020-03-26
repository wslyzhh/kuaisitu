<!-- 非业务支付申请 -->
<template>
    <div>
		<div class="search_box flex flex_a_c flex_j_c">
		   <input type="text" v-model="searchData.keywords" @blur="changeSearch" placeholder="搜索订单号">
		</div>
        <div class="customer_list">
            <h2 class="amount">共{{recordTotal}}条</h2>
            <ul class="list">
				<li v-for="(item,index) in showUnBusinessPayList" :key="index">
				    <div class="company flex flex_a_c flex_s_b">
                        <section class="flex flex_a_c">
                            <img class="icon" v-show="0 == item.uba_flag1" src="../../assets/img/audit.png" alt="">
                            <img class="icon" v-show="1 == item.uba_flag1" src="../../assets/img/audit_no.png" alt="">
                            <img class="icon" v-show="2 == item.uba_flag1" src="../../assets/img/audit_yes.png" alt="">
                            <img class="icon" v-show="0 == item.uba_flag2" src="../../assets/img/audit.png" alt="">
                            <img class="icon" v-show="1 == item.uba_flag2" src="../../assets/img/audit_no.png" alt="">
                            <img class="icon" v-show="2 == item.uba_flag2" src="../../assets/img/audit_yes.png" alt="">
                            <img class="icon" v-show="0 == item.uba_flag3" src="../../assets/img/audit.png" alt="">
                            <img class="icon" v-show="1 == item.uba_flag3" src="../../assets/img/audit_no.png" alt="">
                            <img class="icon" v-show="2 == item.uba_flag3" src="../../assets/img/audit_yes.png" alt="">
                            <h2 class="name">{{item.uba_function}}</h2>
                            <div v-show="false == item.uba_isConfirm" class="lock-status">未支付</div>
                            <div v-show="true == item.uba_isConfirm" class="lock-status green">已支付</div>
                        </section>
						<section class="operation_icon flex">
				            <span @click.prevent.stop="edit(item.uba_id)"></span>
				            <span @click.prevent.stop="del(item.uba_id)"></span>
				        </section>
				    </div>
				    <div class="message flex flex_a_c flex_s_b">
				        <div class="message_list flex">
				            <span v-show="item.uba_oid">{{item.uba_oid}}</span>
				            <span>{{item.uba_money}}</span>
				        </div>
				    </div>
				</li>
            </ul>
			<div class="loadmore" @click="loadNextPage" v-show="pageTotal > searchData.pageIndex">
				点击加载更多
			</div>
        </div>
        <top-nav title="非业务支付申请" text='新增' @rightClick="add"></top-nav>
    </div>
</template>

<script>
// import tabList from '../../components/tab.vue'
import {
	mapActions,
	mapState
} from 'vuex'

export default {
    name:"",
    data() {
       return {
        //    topTablist:['待审核','已审核','已支付'],  
		   showUnBusinessPayList:[],
		   pageTotal:9,
		   recordTotal:9,
		   searchData:{
			   pageIndex:0,
			   pageSize:10,
			   keywords:'',
			//    uba_flag:0,
			//    uba_isConfirm:'',
			   managerid:0     // TODO: 测试用，后面注意修改
		   }
       };
    },
    components: {
        // tabList
    },
    computed: {
		...mapState(            
            {
            userInfo: state => state.user.userInfo
        })     
	},
    created(){
    },
    mounted() {
		this.newUnbusinessManage()
    },
    methods: {
		...mapActions([
			'getUnBusinessPayList',
			'getUnBusinessPayDel'
		]),	
        // changeTab(index){
        //     if(index == 0){
        //         this.searchData.uba_flag = 0
        //         this.searchData.uba_isConfirm=''
		// 		this.newUnbusinessManage()
        //     }
        //     else if(index==1){
        //         this.searchData.uba_flag = 1
        //         this.searchData.uba_isConfirm=''
		// 		this.newUnbusinessManage()
        //     }
        //     else if(index==2){
        //         this.searchData.uba_flag = ''
        //         this.searchData.uba_isConfirm=1
		// 		this.newUnbusinessManage()
		// 	}
        // },
        // changeActive(actives){
		// 	let _this = this
		// 	actives.map((item) => {
		// 		_this.searchData[item.key] = item.value;
		// 	})
		// 	this.newUnbusinessManage()
        // },
		loadNextPage(){
			this.UnBusinessPayList()
		},
		changeSearch(){
		    this.newUnbusinessManage()
		},
		UnBusinessPayList(){
			let _this = this
			_this.searchData.pageIndex++
			_this.searchData.managerid =_this.userInfo.id
			_this.ddSet.showLoad()
			_this.getUnBusinessPayList(_this.searchData).then(function(res){
				_this.ddSet.hideLoad()
				//console.log(res.data)
				if(res.data.msg){
					_this.ddSet.setToast({text:res.data.msg})
					return
				}
				if(_this.searchData.pageIndex < 2){
					_this.showUnBusinessPayList = res.data.list;
					_this.recordTotal = res.data.pageTotal
					_this.pageTotal = Math.ceil(_this.recordTotal / _this.searchData.pageSize)
				}
				else{
					_this.showUnBusinessPayList = _this.showUnBusinessPayList.concat(res.data.list);
				}
			})
		},
		newUnbusinessManage(){
			let _this = this
			_this.searchData.pageIndex = 0;
			_this.UnBusinessPayList()
		},	
        add(){
            this.$router.push({path:'/UnBusinessPayAdd',query:{paytype:1,payfunction:1,type:'add'}})
        },	
        edit(params){
            this.$router.push({path:'/unBusinessPayEdit',query:{uba_id:params,type:'EDIT'}})
        },
		del(_id){
			let _this = this;
			_this.ddSet.setConfirm('确定要删除吗？').then(res=>{
				if(0 == res.buttonIndex){
					_this.ddSet.showLoad()
					_this.getUnBusinessPayDel({
						uba_id:_id,
						managerid:_this.searchData.managerid
					}).then((res) => {
						_this.ddSet.hideLoad()
						if (res.data.status) {
							_this.ddSet.setToast({text:'删除成功'})
							_this.showUnBusinessPayList = _this.showUnBusinessPayList.filter(function(item){
								return item.uba_id != _id
							})
						}
						else{
							_this.ddSet.setToast({text:res.data.msg})
						}
					})
				}
			})
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

