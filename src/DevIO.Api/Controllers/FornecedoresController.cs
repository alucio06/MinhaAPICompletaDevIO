﻿using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorService _fornecedorService;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IMapper _mapper;

        public FornecedoresController(IFornecedorRepository fornecedorRepository, IMapper mapper,
            IFornecedorService fornecedorService, INotificador notificador, IEnderecoRepository enderecoRepository) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
            _fornecedorService = fornecedorService;
            _enderecoRepository = enderecoRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<FornecedorViewModel>> ObterTodos()
        {
            var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

            return fornecedores;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
        {
            var fornecedor = await ObterFornecedorProdutosEndereco(id);

            if (fornecedor == null) return NotFound();

            return fornecedor;
        }

        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorViewModel));

            return CustomResponse(fornecedorViewModel);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado na query");
                return BadRequest();
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorViewModel));

            return CustomResponse(fornecedorViewModel);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorEndereco(id);

            if (fornecedorViewModel == null) return NotFound();

            await _fornecedorService.Remover(id);

            return CustomResponse();
        }

        [HttpGet("obter-endereco/{id:guid}")]
        public async Task<ActionResult<EnderecoViewModel>> ObterEnderecoPorId(Guid id)
        {
            var enderecoViewModel = _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));

            if (enderecoViewModel == null) return NotFound();

            return enderecoViewModel;
        }

        [HttpPut("atualizar-endereco/{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoViewModel enderecoViewModel)
        {
            if (id != enderecoViewModel.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado na query");
                return BadRequest();
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(enderecoViewModel));

            return CustomResponse(enderecoViewModel);
        }

        private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }

        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }
    }
}