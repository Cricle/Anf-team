import { Injectable } from "@angular/core";
import { ComicApiService } from "./comic-api.service";
import { EntityResult, RSAKeyIdentity } from "./model";
import { Observable } from 'rxjs';

const TK:string="ANF-TOKEN";

@Injectable({
    providedIn: 'root'
})
export class UserManager {
    
    private identity:RSAKeyIdentity;
    constructor(private api:ComicApiService){

    }
    public isLogin():boolean{
        const val=this.getCookie(TK);
        return val!=null;
    }
    private getCookie(name:string):string{
        var strcookie = document.cookie;//获取cookie字符串
        var arrcookie = strcookie.split("; ");//分割
        //遍历匹配
        for ( var i = 0; i < arrcookie.length; i++) {
            var arr = arrcookie[i].split("=");
            if (arr[0] == name){
                return arr[1];
            }
        }
        return null;
    }
    
    public login(userName:string,pwd:string):Observable<EntityResult<string>>{
        if(this.identity){
           return this.coreLogin(userName,pwd); 
        }
        return new Observable<EntityResult<string>>(x=>{
            this.api.flushKey().subscribe(y=>{
                this.identity=y.data;
                this.coreLogin(userName,pwd).subscribe(z=>{
                    x.next(z);
                });
            });
        });
    }
    private coreLogin(userName:string,pwd:string):Observable<EntityResult<string>>{
        const pwdHash=this.api.encrypt(this.identity.key,pwd);
        return this.api.login(userName,pwdHash.toString(),this.identity.identity);
    }
}