<!-- 登录 -->
<template>
    <div class="register">
        <div class="logo">
            <img src="../assets/img/logo.png" alt="">
        </div>
        <div class="from">
            <input type="text" v-model="fromData.username" placeholder="请输入员工账号">
            <input type="password" v-model="fromData.password" placeholder="请输入密码">
            <input class="from_btn" type="button" value="登录" @click="submit">
        </div>
        <top-nav title="登录"></top-nav>
    </div>
</template>

<script>
import {mapActions,mapState} from 'vuex'
export default {
    name:"",
    data() {
       return {
           fromData:{}
       };
    },
    components: {},
    computed: {
        ...mapState({
            corpId:state => state.user.corpid,
        })
    },
    created(){
        //console.log(this.userInfo.id)
        //this.powers = this.powerList.map(item => item.urp_code)
    },
    mounted() {

    },
    methods: {
        ...mapActions([
            'getBinding',
            'getUserName'
        ]),
        submit(){
            if(!this.fromData.username){
                this.ddSet.setToast({text:'请输入员工账号'})
                return
            }
            if(!this.fromData.password){
                this.ddSet.setToast({text:'请输入员工密码'})
                return
            }

            //huaigui.y注释20190727
            // console.log(this.corpId)
            // let {username,password} = this.fromData
            // let params = {
            //     username
            // }
            // this.getUserName(params).then(res => {
            //     if(res.data.status){
            //         this.$router.push({path:"/home"})
            //     }else{
            //         this.ddSet.setToast({text:res.data.msg})
            //     }
            // })
            //huaigui.y注释20190727

            this.ddSet.infoCode(this.corpId).then(res => {
                let code = res.code
                let {username,password} = this.fromData
                let params = {
                    username,
                    password,
                    code
                }
                this.getBinding(params).then(res => {
                    let _data = res.data
                    console.log('绑定：'+res.data.model)
                    if(!res.data.status){
                        this.ddSet.setToast({text:res.data.msg})
                    }else{
                        this.ddSet.setToast({text:'绑定成功'}).then(res => {
                            this.$store.commit('setUser',_data.model)
                            console.log('用户'+ _data.model)
                            // console.log(res.data)
                            this.$router.push({path:"/home"})
                        })
                    }
                })
            })
        }
    },
}
</script>

<style scoped lang='scss'>
    .logo{
        text-align: center;
        margin-top: 1rem;
        img{
            width: 4.32rem;
        }
    }
    .from{
        text-align: center;
        margin-top: 1rem;
        input{
            width: 6.3rem;
            height: .8rem;
            background-color: #FFF;
            border: 1px solid #dadada;
            border-radius: 15px;
            text-indent: 20px;
            font-size: .24rem;
            margin-bottom: .5rem;
        }
        .from_btn{
            text-indent: 0;
            border: none;
            background-color: #1eb9ee;
            color: #FFF;
            font-size: .24rem;
        }
    }
    
</style>
