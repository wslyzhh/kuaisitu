<template>
    <div class="">
        <div class="index_top">
            <div class="banner">
                
            </div>
            <div class="inform_list">
                <div class="icon"></div>
                <ul class="list">
                    <li v-for="(item,index) in prizeList" :key="index" v-show="activeIndex == index">
                        {{item.me_title}}
                    </li>
                </ul>
                <router-link class="more" tag="span" :to="{path:'/messageList'}">更多</router-link>
            </div>
        </div>
        <div class="index_nav_list">
            <div class="nav" v-for="(nav,navIndex) in navList" :key="navIndex">
                <h2 class="title">{{nav.title}}</h2>
                <ul class="list flex flex_wrap">
                    <router-link v-if="showModules(item.code?item.code:'')" v-for="(item,index) in nav.list" :key="index" tag="li" :to="'/'+item.link" 
                    class="c_flex flex_a_c flex_j_c">
                        <div class="icon">
                            <img :src="item.imgUrl" alt="">
                            <div v-if="item.name==='业务审批'">
                                <span v-show="aduitCount1>0" class="num">{{aduitCount1}}</span>
                            </div>
                            <div v-else-if="item.name==='业务支付审核'">
                                <span v-show="aduitCount2>0" class="num">{{aduitCount2}}</span>
                            </div>
                            <div v-else-if="item.name==='非业务支付审核'">
                                <span v-show="aduitCount3>0" class="num">{{aduitCount3}}</span>
                            </div>
                            <div v-else-if="item.name==='发票审核'">
                                <span v-show="aduitCount4>0" class="num">{{aduitCount4}}</span>
                            </div>
                            <div v-else-if="item.name==='预付款审批'">
                                <span v-show="aduitCount5>0" class="num">{{aduitCount5}}</span>
                            </div>
                        </div>
                        <p class="name">{{item.name}}</p>
                    </router-link>
                </ul>
            </div>
        </div>
        <top-nav title="快思图商务会展"></top-nav>
    </div>
</template>

<script>
import menu from '../assets/js/indexMenu'
import {mapState,mapActions,mapMutations} from 'vuex'
export default {
    data() {
        return {
            prizeList:[],
            activeIndex:0,
            navList:[],
            powers:[],
            aduitCount1:0,
            aduitCount2:0,
            aduitCount3:0,
            aduitCount4:0,
            aduitCount5:0
        };
    },
    components: {},
    computed: {
        ...mapState({
            userInfo: state => state.user.userInfo,
            corpid:state => state.user.corpid,
            powerList:state => state.user.userInfo.RolePemissionList
        })
    },
    created(){
        //获取用户信息
        // this.userid('11')
        let _this=this
        // console.log('2:'+this.corpid);
        // _this.ddSet.infoCode(_this.corpid).then(res => {
        //     console.log(res.code);
        //     _this.userid(res.code)
        // })
        console.log('用户ID：'+ _this.userInfo.id)
        _this.getAduitCount({managerid:_this.userInfo.id}).then(res => {
            if(res.data.status==1){
                _this.aduitCount1 = res.data.count1
                _this.aduitCount2 = res.data.count2
                _this.aduitCount3 = res.data.count3
                _this.aduitCount4 = res.data.count4
                _this.aduitCount5 = res.data.count5
            }
        })
    },
    mounted() {
        //获取菜单列表        
        this.navList = menu
        setInterval(() => {
            if(this.activeIndex < this.prizeList.length-1) {
                this.activeIndex += 1;
            } else {
                this.activeIndex = 0;
            }
        }, 5000);
        //console.log('用户ID：'+ this.userInfo.id)
        this.powers = this.powerList.map(item => item.urp_code)
    },
    methods: {
        ...mapActions([
            'getUserId',
            'getUserName',
            'getCorpid',
            'getMessageList',
            'removeBinding',
            'getAduitCount'
        ]),
        showModules(arr){
            if(!arr){
                return true
            }
            for (let i in arr) {
                for (let j in this.powers) {
                    if (this.powers[j] === arr[i])return true;
                }
            }
            return false
        },
        getMessage(_id){
            this.getMessageList({pageIndex:1,pageSize:10,managerid:_id,isRead:'False'}).then(res => {
                if(res.data.msg){
					this.ddSet.setToast({text:res.data.msg})
					return
				}
				else{
                    this.prizeList = res.data.list
				}
            })
        },
        // userid(_code){
        //     this.getUserId({code:_code}).then(res => {
        //         console.log(res.data.status)
        //         if(res.data.status == 3){
        //             this.$store.commit('setUser',res.data.model)
        //             this.getMessage(this.userInfo.id)
        //             this.powers = this.powerList.map(item => item.urp_code)
        //             let {data} = res.data.model
        //             let params = {
        //                 username:data.user_name
        //             }
        //             this.getUserName(params).then(res => {
        //                 this.$store.commit('setBinding',res.data.status)
        //                 if(res.data.status === 1){
        //                     this.ddSet.setToast({text:'已绑定'})
        //                 }else{
        //                     this.ddSet.setToast({text:res.data.msg})
        //                 }
        //             })
        //         }else if(res.data.status == 2){
        //             this.$router.push({path:'/register'})
        //         }else{
        //             this.ddSet.setToast({text:res.data.msg})
        //         }
        //     })
        // },
        // signOut(){
        //     console.log(this.corpid)
        //     this.ddSet.infoCode(this.corpid).then(res => {
        //         let _code = res.code
        //         this.removeBinding({code:_code}).then(res => {
        //             if(res.data.status){
        //                 this.ddSet.setToast({text:'您已解除钉钉授权绑定'}).then(res => {
        //                         this.$router.push({path:"/register"})
        //                 })
        //             }else{
        //                 this.ddSet.setToast({text:res.data.msg})
        //             }
        //         })
        //     })
        // }
    },
}
</script>
<style scoped lang="scss">
@import '../assets/css/var.scss';
    .index_nav_list{
        padding: .2rem 0;
        .title{
            font-size: .3rem;
            color: #0d1015;
            padding: .2rem .3rem;
        }
        li{
            width: 25%;
            margin-top: .2rem;
            .icon{
                position: relative;
                width: 1rem;
                height: 1rem;
                img{
                    width: 100%;
                    height: 100%;
                }
                .num{
                    position: absolute;
                    top: -.2rem;
                    right: -.2rem;
                    line-height: 100%;
                    height: .3rem;
                    min-width: .2rem;
                    text-align: center;
                    background-color: red;
                    padding: .03rem .08rem;
                    color: #FFF;
                    border-radius: 20px;
                    font-size: 12px;
                }
            }
            .name{
                font-size: .24rem;
                line-height: .6rem;
                color: $fontSizeColor;
            }
        }
    }
    .index_top{
        margin-top: .27rem;
    }
    .index_top .banner{
        width: 6.92rem;
        height: 2.75rem;
        background-color: #f1f1f1;
        border-radius: .08rem;
        margin: 0 auto;
        background-image: url('../assets/img/bananer.png');
        background-size:6.92rem 2.75rem;
    }
    .inform_list{
        display: flex;
        margin:0 .3rem;
        padding: .3rem 0;
        border-bottom: 1px solid #f1f1f1;
    }
    .inform_list .more{
        font-size: .28rem;
        color: #5193f3;
        line-height: .4rem;
    }
    .inform_list .icon{
        height: .4rem;
        width: .4rem;
        margin-right: .2rem;
        background-image: url('../assets/img/horn.png');
        background-repeat: no-repeat;
        background-size: cover;
    }
    .inform_list .list{
        flex: 1;
        height: .4rem;
        color: #787b80;
        font-size: .28rem;
        overflow: hidden;
    }
    .inform_list .list li{
        line-height: .4rem;
    }
</style>
