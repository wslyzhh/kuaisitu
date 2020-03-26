<!-- 客户管理首页 -->
<template>
    <div>
        <tab-list :tabList="topTabList" @on-tab="changeTab"></tab-list>
        <div class="search_box flex flex_a_c flex_j_c">
           <input type="text" v-model="searchData.keywords" @input="changeSearch" placeholder="搜索消息标题或内容">
        </div>
        <div class="customer_list">
            <h2 class="amount">共{{recordTotal}}条</h2>
            <ul class="list">
                <router-link tag="li" :to="{path:'/messageDetails',query: {me_id:item.me_id,isRead:item.me_isRead}}" v-for="(item,index) in showMessageList" :key="index">
                    <div class="company flex flex_a_c flex_s_b">
                        <section class="flex flex_a_c">
                            <h2 class="name">{{item.me_title}}</h2>
                        </section>
                        <section class="operation_icon flex">
                            <span  @click.prevent.stop="delmessage(item.me_id)"></span>
                        </section>
                    </div>
                </router-link>
            </ul>
            <div class="loadmore" @click="loadNextPage" v-show="pageTotal > searchData.pageIndex">
				点击加载更多
			</div>
        </div>
        <top-nav title="个人消息" ></top-nav>
    </div>
</template>

<script>
import tabList from '../../components/tab.vue'
import {mapActions,mapState} from 'vuex'

export default {
    name:"",
    data() {
       return {
           topTabList:['未读','已读'],
           showMessageList:[],
           list:[],
           pageTotal:9,
           recordTotal:9,
           
		   searchData:{
			    pageIndex:1,
                pageSize:10,
                keywords:'',
                isRead:0,
                managerid:0
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
        this.newrmessageList()
    },
    methods: {
        ...mapActions([
            'getMessageList',
            'deteleMessage'
        ]),
        loadNextPage(){
			this.messageList()
		},
        messageList(){
            let _this = this
            _this.searchData.pageIndex++
            _this.searchData.managerid=this.userInfo.id
            this.getMessageList(_this.searchData).then(res => {
                if(res.data.msg){
					_this.ddSet.setToast({text:res.data.msg})
					return
				}
				if(_this.searchData.pageIndex < 2){
					_this.showMessageList = res.data.list;
					_this.recordTotal = res.data.pageTotal
					_this.pageTotal = Math.ceil(_this.recordTotal / _this.searchData.pageSize)
				}
				else{
					_this.showMessageList = _this.showMessageList.concat(res.data.list);
				}
            })
        },
		newrmessageList(){
			let _this = this
			_this.searchData.pageIndex = 0;
			_this.messageList()
		},
        changeSearch(){
            this.newrmessageList()
        },
        changeTab(index) {
            this.searchData.isRead = index
				this.newrmessageList()
        },
        delmessage(_id){
            let _this = this;
            _this.ddSet.setConfirm('确定要删除消息吗？').then(res=>{
                if(0 == res.buttonIndex){
                    _this.ddSet.showLoad()
                    _this.deteleMessage({
                        me_id:_id
                    }).then((res) => {
                        _this.ddSet.hideLoad()
                        if (res.data.status) { 
                            _this.ddSet.setToast({text:'删除成功'})
							_this.showMessageList = _this.showMessageList.filter(function(item){
								return item.me_id != _id
							})                         
                        }
                        else{
                            _this.ddSet.setToast({text:res.data.msg})
                        }
                    })
                }
            })
		},
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
    .loadmore{text-align:center;padding:25px;font-size:.3rem;color:#888;background-color: #ededed}
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
                        background-image: url('../../assets/img/delete.png');
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


