<template>
  <div id="app">
    <!-- <div v-if="isHome"> -->
      <router-view/>
    <!-- </div> -->
  </div>
</template>

<script>
import {mapState,mapActions,mapMutations} from 'vuex'
export default {
  name: 'App',
  data(){
      return {
        isHome:true
      }
  },
  computed:{
    ...mapState({
      corpid:state => state.user.corpid
    })
  },
  created(){
    // this.userid('11')
  },
  mounted(){
    //获取企业ID
    this.getCorpid().then(res => {
        this.$store.commit('setCorpid',res.data.corpid)
        //获取用户信息
        console.log('1:' + this.corpid)
        this.ddSet.infoCode(this.corpid).then(res => {
            console.log(res)
            this.userid(res.code)
        })
    })
  },
  methods: {
    ...mapActions([
        'getUserId',
        'getUserName',
        'getCorpid'
    ]),
    userid(code){
      console.log(code)
      this.getUserId({code}).then(res => {
          console.log('状态码：'+ res.data.status)
          if(res.data.status == 3){
              this.isHome = true
              //let {data} = res.data
              console.log('用户1：'+ res.data.model)
              this.$store.commit('setUser',res.data.model)
              this.$router.push({path:"/home"})
              // let params = {
              //     username:data.user_name
              // }
              // this.getUserName(params).then(res => {
              //     this.$store.commit('setBinding',res.data.status)
              //     if(res.data.status === 1){
              //         this.ddSet.setToast({text:'已绑定'})
              //     }else{
              //         this.ddSet.setToast({text:res.data.msg})
              //     }
              // })
          }else if(res.data.status == 2){
              this.isHome = true
              this.$router.push({path:'/register'})
          }else{
              this.ddSet.setToast({text:res.data.msg})
          }
      })
    }
  },
}
</script>

<style lang="scss">
  @import '@/assets/css/index.scss';
</style>
