<!-- 新增付款通知 -->
<template>
    <div class="body_gery">
        <ul class="form_list">
            <li class="flex flex_a_c">
                <label class="title"><span>订单号</span></label>
                <h3 class="hint_1">{{addData.rpd_oid}}</h3>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="changeClient">
                <label class="title"><span class="must">付款对象</span></label>
                <input type="text" :value="clientName" readonly>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">付款金额</span></label>
                <input type="text" v-model="addData.rpd_money" placeholder="请输入付款金额">
            </li>
            <li class="flex flex_a_c flex_s_b" @click="selectDate">
                <label class="title"><span class="must">预付日期</span></label>
                <input type="text" v-model="addData.rpd_foredate" readonly placeholder="请选择预付日期">
                <div class="icon_right time"></div>
            </li>
            <li class="li_auto flex" @click="selectBank">
                <label class="title"><span class="must">银行账号</span></label>
                <textarea class="bankContent" readonly :value="bankName" placeholder="请选择客户"></textarea>
            	<div class="icon_right arrows_right"></div>
            </li>
            <li class="li_auto flex">
                <label class="title"><span>付款内容</span></label>
                <textarea class="rpContent" v-model="addData.rpd_content" placeholder="请输入付款内容"></textarea>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>附件</span></label>
				<div class="icon_right accessory">
					<input type="file" multiple="multiple" ref="FileUp" @change="upFile()">
				</div>
            </li>
			<li class="flex flex_a_c flex_s_b" v-for="(f,index) in files" :key="index">
				<a :href="'/' + f.f_filePath">{{f.f_fileName}}</a>
				<span>{{f.f_size}}K</span>
			    <div class="icon_right delete"  
				@click="delOrderFile(f.f_id,f.f_fileName)"></div>
			</li>
        </ul>
        <choose :show.sync="showChoose" :type="chooseType" :list="chooseList" @on-affirm="activeChoose"></choose>
        <top-nav :title='type == "add" ? "添加付款明细":"查看付款明细"' :text='"保存"' @rightClick="submit"  ></top-nav>
    </div>
</template>
<script>
import {mapActions,mapState} from 'vuex'
import choose from '@/components/choose.vue'
export default {
    name:"",
    data() {
       return {
           addData:{},
           clientList:[],
           clientName:'请选择付款对象',
           clientId:0,
           type:'',
           files:[],
           fileData:'',
           bankID:0,
           bankName:'',
           chooseList:[],
           chooseType:1,
           showChoose:false
       };
    },
    components: {choose},
    computed: {
        ...mapState(            
            {
            selectClientArray:state => state.addOrders.selectClientArray,
            userInfo: state => state.user.userInfo
        })
    },
    created(){
        let {type,oID} = this.$route.query
        this.addData.rpd_oid=oID
        this.type=type
    },
    mounted() {
        this.clientCallBack(this.selectClientArray)
    },
    methods: {
        ...mapActions([
            'getAllCustomer',
            'getAddReceiptPayDetail',
            'getUpLoadFile',
            'delFile',
            'getBankList'
        ]),
        submit(item){ //提交
            // console.log('2:'+this.fileData)           
            // var data = new FormData();
            // Object.values(this.fileData).map((item,index) => { 
            //     data.append("file",item)                
            // }) 
            // data.append("type",2)
            // data.append("keyID",117)
            // data.append("fileType",1)
            // data.append("managerid",this.userInfo.id)
            // this.getUpLoadFile(data).then(res => {
            //     if(1 == res.data.status){
            //         this.ddSet.setToast({text:'添加成功'}).then(res => {
            //             this.$router.go(-1)
            //         })
            //     }
            //     else{
            //         this.ddSet.setToast({text:res.data.msg})
            //     }
            // })
            //this.addData.rpd_foredate='2019-07-11'
            if(!this.clientId){
                this.ddSet.setToast({text:'请选择付款对象'})
                return
            }
            if(!this.addData.rpd_money){
                this.ddSet.setToast({text:'请填写付款金额'})
                return
            }
            if(!this.addData.rpd_foredate){
                this.ddSet.setToast({text:'请填写预付日期'})
                return
            }            
            this.addData.rpd_type=false
            this.addData.rpd_cid=this.clientId
            this.addData.managerid = this.userInfo.id //测试ID
            this.ddSet.showLoad()
            if(this.type == 'add'){
                let _this=this
                this.getAddReceiptPayDetail(_this.addData).then(res => {
                    if(res.data.status){
                        //上传附件                                      
                        if(_this.fileData.length > 0){
                            var data = new FormData();
                            Object.keys(_this.fileData).map((item,index)=>{
                                data.append("file",_this.fileData[index])                
                            }) 
                            data.append("type",2)
                            data.append("keyID",res.data.rpd_id)
                            data.append("fileType",1)
                            data.append("managerid",_this.addData.managerid)
                            this.getUpLoadFile(data).then(res => {                                
                                _this.ddSet.hideLoad()
                                if(res.data.status){
                                    _this.ddSet.setToast({text:'添加成功'}).then(res => {
                                        _this.$router.go(-1)
                                    })
                                }
                                else{
                                    _this.ddSet.setToast({text:res.data.msg})
                                }
                            })
                        }
                        else{
                            _this.ddSet.hideLoad()
                            _this.ddSet.setToast({text:'添加成功'}).then(res => {
                                _this.$router.go(-1)
                            })
                        }
                    }else{                         
                        _this.ddSet.hideLoad()
                        _this.ddSet.setToast({text:res.data.msg})
                    }
                }).catch(err => {
                    _this.ddSet.hideLoad()
                })
            }
        },
        changeClient(){ //选择客户
			let _this = this;
			// _this.clientName = '123123'
			// return;
			this.$router.push({ path: '/addOrders/customerSelect', query: { selected_id: _this.clientId }})
        },
		clientCallBack:function(_selectData){
			if(_selectData.length){
				this.clientName = _selectData[0].name;
				this.clientId = _selectData[0].id;
			}
			else{
				this.clientName = '请选择';
				this.clientId = 0;
			}
		},
        selectDate(){ //活动日期
            this.ddSet.setDatepicker().then(res => {
                    this.$set(this.addData,'rpd_foredate',res.value)
                })
        },
        activeChoose(items){
    		let _this = this
    		if(items.length < 1){
				this.ddSet.setToast({text:'请正确选择'})
    			return;
            }
            console.log(items)
    		_this.$set(_this.addData,'bankID',items[0].cb_id)
            _this.$set(_this.addData,'bankName',items[0].cbname)
            _this.bankName =items[0].cbname
        },
        selectBank(){
            let _this = this
            //_this.clientId=38
    		if(_this.clientId < 1){
				this.ddSet.setToast({text:'请先选择客户'})
    			return
            }
            _this.getBankList({cid:_this.clientId,managerid:_this.userInfo.id}).then(res => {
    			let source = []
    		    res.data.map((item,index) => {
    				if(item.cbname){
    				    _this.$set(item,'name',item.cbname)
    				}
    				if(this.addData.bankID == item.cb_id ){
    					_this.$set(item,'isChecked',true)
    				}
    				source.push(item)
                })
    			_this.chooseList = source
    		    _this.showChoose = true
    		})
        },
        upFile(e){
            let _this = this
            _this.fileData = _this.$refs.FileUp.files
            Object.keys(_this.fileData).map((item,index)=>{
                _this['files'].push({
                    f_fileName:_this.fileData[index].name,
                    f_id:0
                })
            }) 
            
            // data.append("type",2)
            // data.append("keyID",117)
            // data.append("fileType",1)
            // data.append("managerid",this.userInfo.id)
            // this.getUpLoadFile(data).then(res => {
            //     if(1 == res.data.status){
            //         this.ddSet.setToast({text:'添加成功'}).then(res => {
            //             this.$router.go(-1)
            //         })
            //     }
            //     else{
            //         this.ddSet.setToast({text:res.data.msg})
            //     }
            // })
        },
		delOrderFile(_fid,_fileName){
            let _this = this;
            this.ddSet.setConfirm('确定要删除《'+_fileName+'》文件吗？').then(res=>{
                if(0 == res.buttonIndex){
                    _this.ddSet.showLoad()
                    if(_fid){
                        _this.delFile({
					   	fileID:_fid,
					   	type:2,
					   	managerid:_this.addData.managerid
                        }).then(res => {
                            _this.ddSet.hideLoad()
                            if(1 == res.data.status){
                                _this.ddSet.setToast({text:'删除文件成功'})
                                _this['files'] = _this['files'].filter(function(item){
                                    return item.f_id != _fid
                                })
                            }
                            else{
                                _this.ddSet.setToast({text:res.data.msg})
                            }
                        }).catch(err => {
                            _this.ddSet.hideLoad()
                        })
                    }
                    else{
                        _this.ddSet.hideLoad()
                        _this['files'] = _this['files'].filter(function(item){
                            return item.f_id != _fid
                        })
                    }
                }
            }) 			
		}
    },
}
</script>

<style scoped lang='scss'>

    .rpContent{
        height: 1.75rem;
    }
    .bankContent{
        height: 0.85rem;
    }
</style>