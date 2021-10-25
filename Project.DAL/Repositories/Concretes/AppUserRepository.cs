﻿using Microsoft.AspNetCore.Identity;
using Project.DAL.Context;
using Project.DAL.Repositories.Abstracts;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Repositories.Concretes
{
    public class AppUserRepository : BaseRepository<AppUser>, IAppUserRepository
    {
        //Sizin kendinize özel Crud işlemlerinin yine olması gerekir....Ancak unutmayın ki Identity yapısının özel şifrelemeler ve yetkilendirmeleri icin hazır async metotları vardır...BU metotların kullanımı icin de ekstra olarak bu Repository'de ayrı alanlar acmak en dogrusudur...Bu metotlar Manager sınıfları icerisinde bulundurur(UserManager,SignInManager Identity'de gömülü olan sınıflardır). Bu sınıflar Dependency Injection ile calısırlar...Ve constructor based injection yapılabilir...

        UserManager<AppUser> _userManager;
        SignInManager<AppUser> _signInManager;

        public AppUserRepository(MyContext db, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //Özel bir Identity Metodu async seklinde tanımlanmalıdır...Cünkü siz burada sonucta bir API kullanıyorsunuz...Ve bu API requestlerinin bloklanmadan devam edebilmesi icin await keyword'unu kullanmamız gerekir...Bir metot icerisinde await keyword'unu kullanabilmeniz icin o metodun async tanımlanması gerekir ve Task döndürmesi gerekir...

        public async Task<bool> AddUser(AppUser item)
        {
            //Sadece Asenkron olarak yaratılmıs (async marklı) metotlar icerisinde await kullanabilirsiniz...

            IdentityResult result = await _userManager.CreateAsync(item, item.PasswordHash);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(item, isPersistent: false); //isPersisten durumu Cookie'de dursun mu durmasın mı
                return true;
            }
            return false;
        }

    }
}
