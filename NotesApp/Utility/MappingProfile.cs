using AutoMapper;
using Entities;
using Shared.DTO;

namespace NotesApp.Utility;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, NewUserRequestDTO>().ReverseMap();
        CreateMap<UserDTO, User>().ReverseMap();
        CreateMap<User, UserDTO>().ReverseMap();


        CreateMap<Note,NoteRequestDTO>().ReverseMap();
        CreateMap<NoteDTO, Note>().ReverseMap();
    }  
}
